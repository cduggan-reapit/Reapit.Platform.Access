// using FluentAssertions;
// using Reapit.Platform.Access.Domain.Entities;
// using Reapit.Platform.Common.Providers.Temporal;
//
// namespace Reapit.Platform.Access.Domain.UnitTests.Entities.Internal;
//
// public class OrganisationUserTests
// {
//     /*
//      * Ctor
//      */
//
//     [Fact]
//     public void Ctor_PopulatesInstance_FromParameters()
//     {
//         var user = new User("user-id", "user-name", "user-email");
//         var organisation = new Organisation("organisation-id", "organisation-name");
//
//         var timeFixture = new DateTimeOffset(2024, 11, 12, 13, 47, 11, TimeSpan.Zero);
//         using var _ = new DateTimeOffsetProviderContext(timeFixture);
//
//         var sut = new OrganisationUser(organisation, user);
//         sut.Organisation.Should().BeSameAs(organisation);
//         sut.User.Should().BeSameAs(user);
//         sut.OrganisationId.Should().Be(organisation.Id);
//         sut.UserId.Should().Be(user.Id);
//         sut.DateLastSynchronised.Should().Be(timeFixture);
//     }
//     
//     /*
//      * AsSerializable
//      */
//
//     [Fact]
//     public void AsSerializable_CreatesAnonymousObject_FromInstanceProperties()
//     {
//         var user = new User("user-id", "user-name", "user-email");
//         var organisation = new Organisation("organisation-id", "organisation-name");
//         using var _ = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
//         var entity = new OrganisationUser(organisation, user) { Id = 37 };
//
//         var expected = new { entity.Id, User = entity.UserId, Organisation = entity.OrganisationId, Sync = entity.DateLastSynchronised.UtcDateTime };
//         var actual = entity.AsSerializable();
//         actual.Should().BeEquivalentTo(expected);
//     }
// }