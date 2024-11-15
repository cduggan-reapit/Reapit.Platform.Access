using Reapit.Platform.Access.Core.UnitTests.TestHelpers;
using Reapit.Platform.Access.Core.UseCases;
using Reapit.Platform.Access.Core.UseCases.Organisations;
using Reapit.Platform.Access.Core.UseCases.Organisations.SynchroniseOrganisation;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Organisations.SynchroniseOrganisation;

public class SynchroniseOrganisationCommandValidatorTests
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
        actual.Should().Fail(nameof(SynchroniseOrganisationCommand.Id), CommonValidationMessages.Required);
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
        actual.Should().Fail(nameof(SynchroniseOrganisationCommand.Name), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validate_Fails_WhenNameTooLong()
    {
        var request = GetRequest(name: new string('b', 101));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(request);
        actual.Should().Fail(nameof(SynchroniseOrganisationCommand.Name), OrganisationValidationMessages.NameExceedsMaxLength);
    }
    
    /*
     * Private methods
     */

    private static SynchroniseOrganisationCommandValidator CreateSut() => new();

    private static SynchroniseOrganisationCommand GetRequest(string id = "valid-id", string name = "valid-name")
        => new(id, name);
}