# Organisations Service (v2)

**Repository:** Reapit.Platform.Access

**Namespace:** Reapit.Platform.Access

**Description:** Access management service used to control user access to products and product instances.  
Broadcasts event notifications to a configurable topic.

---

## Service

- This services uses packages exclusive to the [Reapit GitHub NuGet](https://docs.docker.com/desktop/install/windows-install/) 
  feed.  Ensure your NuGet configuration includes this source. 

---

## Testing

### Code Coverage

- Open a terminal window in the root directory

- Get rid of any existing TestResults directories:
```sh
# PowerShell:
Get-Childitem -Include TestResults -Recurse -Force | Remove-Item -Force -Recurse
```

- Run the following command to run the tests with code coverage analysis:
```sh
dotnet test .\src\Reapit.Platform.Access.sln --collect:"XPlat Code Coverage"
```

- Run the following command to create the code coverage report in `./coverage`
```sh
reportgenerator -reports:./src/**/coverage.cobertura.xml -targetdir:coverage -reporttypes:Html -filefilters:-*Migrations*
```

- Open the report (`./coverage/index.html`) in a browser
```sh
# Bash:
open coverage/index.html

# Command Shell:
start "" "coverage/index.html"

# PowerShell:
ii ./coverage/index.html
```

---

## Deployment

### Prerequisites
- Install the [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
- Install the [AWS CDK CLI](https://docs.aws.amazon.com/cdk/v2/guide/getting_started.html#getting_started_install)
- [Configure](#configure-aws-sso-profile) your AWS SSO profile(s)
- Install Docker (either [engine](https://docs.docker.com/engine/install/) or [desktop](https://docs.docker.com/desktop/install/windows-install/))

#### Configure AWS SSO Profile

> SOME DESCRIPTION

- Open a terminal window
- Execute the command `aws configure sso --profile <your-profile-name>`
- Do not enter an SSO Session name
- Enter the start URL (usually `https://reapitgroup.awsapps.com/start#/`)
- Enter the SSO AWS region (e.g. `eu-west-2`)
- Follow the instructions to authorise the session
- Select the account to associate with the profile from the list
- Enter the default client region (e.g. `eu-west-2`)
- Leave the CLI output format blank
- Test the connection by running the example command `aws s3 ls --profile <your-profile-name>` 

### Infrastructure Provisioning

This project provides an AWS CDK project to provision the required infrastructure.  This project can be found in the 
`./cdk` directory. 

- Authenticate the AWS profile using `aws sso login --profile <your-profile-name>`
- Open a terminal window in the `./cdk` directory
- Run `cdk synth --profile <your-profile-name>` to create a CloudFormation template at `./cdk/cdk.out/orgs-v2-sandbox.template.json`
- Run `cdk deploy --profile <your-profile-name>` to deploy the CloudFormation stack to your environment
  - If this is the first deployment in a new environment, make sure you [publish the docker image](#docker-image-publishing)
    once the ECR repository has been created.  If the deployment can't find the image, it will fail and rollback takes 
    quite a long time - you have at least 5 minutes worth of retries before it gives up. 

### Docker Image Publishing

The provisioned infrastructure hosts this service in an ECS cluster using a docker image.  The docker image is hosted 
in an ECR repository.

- Get the URI of the <repository> ECR repository from AWS
- Get the [Personal Access Token (PAT)](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens) to access the Reapit GitHub NuGet feed
- Open a terminal window in the `./src` directory
- Make sure docker is running
  - Try running `docker ps` - if docker is not running, this will let you know
- Authenticate the docker client with AWS
  ```
  aws ecr get-login-password --region eu-west-2 --profile <your-profile-name> | docker login --username AWS --password-stdin <ecr-uri>
  ```
- Build the service docker image
  ```cmd
  docker build -t <repository> --build-arg NUGET_ACCESS_TOKEN=<personal-access-token> .
  ```
- Tag the image
  ```
  docker tag <repository>:latest <ecr-uri>/<repository>:latest
  ```
- Push the image to the container repository
  ```
  docker push <ecr-uri>/<repository>:latest
  ```

### Database Preparation

When setting up a new environment, you will need to apply the database migrations from this project.  As the databases 
are placed in a private subnet, we have to use the VPC bastion host to connect.  The deployed CDK stack Outputs includes
`DatabaseTunnelCommand` which creates a tunnel from your local PC to the database read/write instance.

- Build the application
- Note the Instance ID of the bastion host (`bastion-host-instance-id`)
- Note the contents of the database administrator user secret in Secrets Manager  (`host`, `port`, `dbname`, `username`, 
  and `password`)
- Update `ConnectionStrings:Writer` in `appsettings.Development.json` to: 
  `Server=127.0.0.1; Port=7777; Database=<dbname>; Uid=<username>; Pwd='<password>';`
- Open a terminal window and establish an SSL tunnel to the database server using the `DatabaseTunnelCommand`:
  ```
  aws ssm start-session 
    --target <bastion-host-instance-id> 
    --document-name AWS-StartPortForwardingSessionToRemoteHost 
    --parameters "portNumber"=["<port>"],"localPortNumber"=["7777"],"host"=["<host>"] 
    --region <region> 
    --profile <profile-name>
  ```
- Open a new terminal window in the `./src` directory
- Apply the migrations using 
  ```sh
  # Windows:
  dotnet ef database update --project ./Reapit.Platform.Organisations.Data --startup-project ./Reapit.Platform.Organisations.Api
  
  # Mac:
  dotnet ef database update --project ./Reapit.Platform.Organisations.Data/Reapit.Platform.Organisations.Data.csproj --startup-project ./Reapit.Platform.Organisations.Api/Reapit.Platform.Organisations.Api.csproj
  ```
If this is the first migration in the database, you'll also want to create a database user with limited permissions.
- Connect to the database (we typically use HeidiSQL, but any SQL IDE or the MySQL CLI should be fine)
- Create a new user `CREATE USER <username>@'%' IDENTIFIED BY '<password>';`;
  - I've used [this password generator](https://passwordsgenerator.net/) to generate random passwords
- Grant self-privileges to the new user `GRANT ALL PRIVILEGES ON <username>.* TO <username>@'%';`
- Grant DML permissions on the service database `GRANT INSERT, UPDATE, DELETE, SELECT, EXECUTE ON <database>.* TO '<username>'@'%';`
- Manually create a secret in AWS containing the username and password:
  ![Create Database Secret](docs/_img/AWS_CreateDatabaseSecret.png)
- Update the `database.userSecret.arn` and `database.userSecret.name` keys in the `/Platform/Organisations/v2/Configuration` 
  parameter to reference the newly created user secret.
- It's worth restarting the ECS task(s) now to make sure they're using the restricted user rather than the master credential.
  - To restart the ECS tasks using the CLI run `aws ecs update-service --cluster <cluster name> --service <service name> --force-new-deployment`
  - You can confirm which credentials are in use by checking the logs for `Injecting connection strings from <secret>`  
