using Reapit.Platform.Access.Core.UseCases;
using Reapit.Platform.Access.Core.UseCases.Users;
using Reapit.Platform.Access.Core.UseCases.Users.CreateUser;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Users.CreateUser;

public class CreateUserCommandValidatorTests
{
    [Fact]
    public async Task Validate_Succeeds_WhenRequestValid()
    {
        var request = GetRequest();
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Pass();
    }
    
    /*
     * Id
     */

    [Fact]
    public async Task Validate_Fails_WhenIdIsEmpty()
    {
        var request = GetRequest(id: string.Empty);
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(CreateUserCommand.Id), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validate_Fails_WhenIdIsTooLong()
    {
        var request = GetRequest(id: new string('0', 101));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(CreateUserCommand.Id), UserValidationMessages.IdExceedsMaxLength);
    }
    
    /*
     * Name
     */
    
    [Fact]
    public async Task Validate_Fails_WhenNameIsEmpty()
    {
        var request = GetRequest(name: string.Empty);
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(CreateUserCommand.Name), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validate_Fails_WhenNameIsTooLong()
    {
        var request = GetRequest(name: new string('0', 501));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(CreateUserCommand.Name), UserValidationMessages.NameExceedsMaxLength);
    }
    
    /*
     * Email
     */
    
    [Fact]
    public async Task Validate_Fails_WhenEmailIsEmpty()
    {
        var request = GetRequest(email: string.Empty);
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(CreateUserCommand.Email), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validate_Fails_WhenEmailIsTooLong()
    {
        var request = GetRequest(email: new string('0', 1001));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(CreateUserCommand.Email), UserValidationMessages.EmailExceedsMaxLength);
    }
    
    /*
     * Private methods
     */

    private static CreateUserCommandValidator CreateSut() => new();

    private static CreateUserCommand GetRequest(
        string id = "valid-id", 
        string name = "valid-name",
        string email = "valid-email")
        => new(id, name, email);
}