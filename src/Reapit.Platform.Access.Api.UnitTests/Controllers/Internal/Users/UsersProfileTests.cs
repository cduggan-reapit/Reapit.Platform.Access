using Reapit.Platform.Access.Api.Controllers.Internal.Users.V1;
using Reapit.Platform.Access.Api.Controllers.Internal.Users.V1.Models;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers.Internal.Users;

public class UsersProfileTests
{
    /*
     * User => SimpleUserModel
     */

    [Fact]
    public void UsersProfile_PopulatesSimpleUserModel_FromUserEntity()
    {
        const string id = "id", name = "name", email = "email";
        
        // Put it in BST so that we can check the returned value is in UTC
        var timeFixture = new DateTimeOffset(2024, 9, 11, 12, 32, 14, TimeSpan.FromHours(1));
        using var _ = new DateTimeOffsetProviderContext(timeFixture);

        var entity = new User(id, name, email);
        var expected = new SimpleUserModel(id, name, email, timeFixture.UtcDateTime);

        var sut = CreateSut();
        var actual = sut.Map<SimpleUserModel>(entity);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */

    private static IMapper CreateSut() 
        => new MapperConfiguration(
                cfg => cfg.AddProfile(typeof(UsersProfile)))
            .CreateMapper();
}