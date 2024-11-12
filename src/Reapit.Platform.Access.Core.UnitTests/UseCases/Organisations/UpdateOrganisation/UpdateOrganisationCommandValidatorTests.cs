using Reapit.Platform.Access.Core.UseCases;
using Reapit.Platform.Access.Core.UseCases.Organisations;
using Reapit.Platform.Access.Core.UseCases.Organisations.UpdateOrganisation;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Organisations.UpdateOrganisation;

public class UpdateOrganisationCommandValidatorTests
{
    /*
     * Validate
     */

    [Fact]
    public async Task Validate_Succeeds_WhenRequestValid()
    {
        var request = GetRequest();
        var sut = UpdateSut();
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
        var sut = UpdateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(UpdateOrganisationCommand.Id), CommonValidationMessages.Required);
    }

    /*
     * Name
     */
    
    [Fact]
    public async Task Validate_Fails_WhenNameIsEmpty()
    {
        var request = GetRequest(name: string.Empty);
        var sut = UpdateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(UpdateOrganisationCommand.Name), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validate_Fails_WhenNameTooLong()
    {
        var request = GetRequest(name: new string('b', 101));
        var sut = UpdateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(UpdateOrganisationCommand.Name), OrganisationValidationMessages.NameExceedsMaxLength);
    }
    
    /*
     * Private methods
     */

    private static UpdateOrganisationCommandValidator UpdateSut() => new();

    private static UpdateOrganisationCommand GetRequest(string id = "valid-id", string name = "valid-name")
        => new(id, name);
}