namespace Reapit.Platform.Access.Api.IntegrationTests.TestHelpers;

public abstract class ApiIntegrationTestBase(TestApiFactory apiFactory) : IClassFixture<TestApiFactory>
{
    protected readonly TestApiFactory ApiFactory = apiFactory;

    protected async Task<HttpResponseMessage> SendRequestAsync(
        HttpMethod? method,
        string url, 
        string? version = "1.0",
        object? content = null)
    {
        var client = ApiFactory.CreateClient();
        
        if (version is not null)
            client.DefaultRequestHeaders.Add(Constants.ApiVersionHeader, version);
        
        return method?.Method.ToUpperInvariant() switch
        {
            "GET" => await client.GetAsync(url),
            "POST" => await client.PostAsync(url, content?.ToStringContent()),
            "PUT" => await client.PutAsync(url, content?.ToStringContent()),
            "PATCH" => await client.PatchAsync(url, content?.ToStringContent()),
            "DELETE" => await client.DeleteAsync(url),
            _ => throw new NotImplementedException()
        };
    }
}