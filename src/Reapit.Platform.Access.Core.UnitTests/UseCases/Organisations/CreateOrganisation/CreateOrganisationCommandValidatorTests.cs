using Reapit.Platform.Access.Core.UseCases;
using Reapit.Platform.Access.Core.UseCases.Organisations;
using Reapit.Platform.Access.Core.UseCases.Organisations.CreateOrganisation;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Organisations.CreateOrganisation;

public class CreateOrganisationCommandValidatorTests
{
    /*
     * Validate
     */

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
    public async Task Validate_Fails_WhenIdEmpty()
    {
        var request = GetRequest(id: string.Empty);
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(CreateOrganisationCommand.Id), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validate_Fails_WhenIdTooLong()
    {
        var request = GetRequest(id: new string('a', 101));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(CreateOrganisationCommand.Id), OrganisationValidationMessages.IdExceedsMaxLength);
    }
    
    /*
     * Name
     */
    
    [Fact]
    public async Task Validate_Fails_WhenNameTooLong()
    {
        var request = GetRequest(name: new string('b', 101));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(CreateOrganisationCommand.Name), OrganisationValidationMessages.NameExceedsMaxLength);
    }
    
    /*
     * Private methods
     */

    private static CreateOrganisationCommandValidator CreateSut() => new();

    private static CreateOrganisationCommand GetRequest(string id = "valid-id", string name = "valid-name")
        => new(id, name);
}