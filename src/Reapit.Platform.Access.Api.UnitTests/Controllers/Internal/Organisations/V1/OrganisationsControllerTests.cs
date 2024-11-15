using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;
using Reapit.Platform.Access.Core.UseCases.Organisations.AddOrganisationMember;
using Reapit.Platform.Access.Core.UseCases.Organisations.DeleteOrganisation;
using Reapit.Platform.Access.Core.UseCases.Organisations.RemoveOrganisationMember;
using Reapit.Platform.Access.Core.UseCases.Organisations.SynchroniseOrganisation;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers.Internal.Organisations.V1;

public class OrganisationsControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    
    /*
     * SynchroniseOrganisation
     */
    
    [Fact]
    public async Task UpdateOrganisation_ReturnsNoContent()
    {
        const string id = "id", name = "name";
        var organisation = new Organisation(id, name);
        
        _mediator.Send(Arg.Any<SynchroniseOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(organisation);

        var sut = CreateSut();
        var actual = await sut.SynchroniseOrganisation(id, new SynchroniseOrganisationRequestModel(name)) as NoContentResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(204);
        
        await _mediator.Received(1).Send(
            request: Arg.Is<SynchroniseOrganisationCommand>(command => command.Id == id && command.Name == name),
            cancellationToken: Arg.Any<CancellationToken>());
    }
    
    /*
     * DeleteOrganisation
     */
    
    [Fact]
    public async Task DeleteOrganisation_ReturnsNoContent()
    {
        const string id = "id";
        var organisation = new Organisation(id, "name");
        
        _mediator.Send(Arg.Any<DeleteOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(organisation);

        var sut = CreateSut();
        var actual = await sut.DeleteOrganisation(id) as NoContentResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(204);

        await _mediator.Received(1).Send(
            request: Arg.Is<DeleteOrganisationCommand>(command => command.Id == id),
            cancellationToken: Arg.Any<CancellationToken>());
    }
    
    /*
     * AddOrganisationMember
     */
    
    [Fact]
    public async Task AddOrganisationMember_ReturnsNoContent()
    {
        const string organisationId = "organisation-id", userId = "user-id";

        var sut = CreateSut();
        var actual = await sut.AddOrganisationMember(organisationId, userId) as NoContentResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(204);

        await _mediator.Received(1).Send(
            request: Arg.Is<AddOrganisationMemberCommand>(command => command.OrganisationId == organisationId && command.UserId == userId),
            cancellationToken: Arg.Any<CancellationToken>());
    }
    
    /*
     * RemoveOrganisationMember
     */
    
    [Fact]
    public async Task RemoveOrganisationMember_ReturnsNoContent()
    {
        const string organisationId = "organisation-id", userId = "user-id";
        
        var sut = CreateSut();
        var actual = await sut.RemoveOrganisationMember(organisationId, userId) as NoContentResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(204);

        await _mediator.Received(1).Send(
            request: Arg.Is<RemoveOrganisationMemberCommand>(command => command.OrganisationId == organisationId && command.UserId == userId),
            cancellationToken: Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private OrganisationsController CreateSut()
        => new(_mediator);
}