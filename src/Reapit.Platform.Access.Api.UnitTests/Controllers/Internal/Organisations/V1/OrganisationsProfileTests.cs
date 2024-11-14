using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers.Internal.Organisations.V1;

public class OrganisationsProfileTests
{
    /*
     * Organisation => SimpleOrganisationModel
     */

    [Fact]
    public void OrganisationsProfile_PopulatesOrganisationModel_FromOrganisationEntity()
    {
        const string id = "id", name = "name";
        
        // Put it in BST so that we can check the returned value is in UTC
        var timeFixture = new DateTimeOffset(2024, 9, 11, 12, 32, 14, TimeSpan.FromHours(1));
        using var _ = new DateTimeOffsetProviderContext(timeFixture);

        var entity = new Organisation(id, name);
        var expected = new SimpleOrganisationModel(id, name, timeFixture.UtcDateTime);

        var sut = CreateSut();
        var actual = sut.Map<SimpleOrganisationModel>(entity);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */

    private static IMapper CreateSut() 
        => new MapperConfiguration(
                cfg => cfg.AddProfile(typeof(OrganisationsProfile)))
            .CreateMapper();
}