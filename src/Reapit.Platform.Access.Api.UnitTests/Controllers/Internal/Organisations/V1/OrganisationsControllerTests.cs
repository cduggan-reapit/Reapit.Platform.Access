using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;
using Reapit.Platform.Access.Core.UseCases.Organisations.AddOrganisationMember;
using Reapit.Platform.Access.Core.UseCases.Organisations.CreateOrganisation;
using Reapit.Platform.Access.Core.UseCases.Organisations.DeleteOrganisation;
using Reapit.Platform.Access.Core.UseCases.Organisations.GetOrganisationById;
using Reapit.Platform.Access.Core.UseCases.Organisations.RemoveOrganisationMember;
using Reapit.Platform.Access.Core.UseCases.Organisations.UpdateOrganisation;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers.Internal.Organisations.V1;

public class OrganisationsControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile(typeof(OrganisationsProfile)))
        .CreateMapper();

    /*
     * GetOrganisationById
     */

    [Fact]
    public async Task GetOrganisationById_ReturnsOk_WithSimpleOrganisationModel()
    {
        const string id = "id";
        var organisation = new Organisation(id, "name");
        var expected = _mapper.Map<SimpleOrganisationModel>(organisation);

        _mediator.Send(Arg.Is<GetOrganisationByIdQuery>(query => query.Id == id), Arg.Any<CancellationToken>())
            .Returns(organisation);

        var sut = CreateSut();
        var actual = await sut.GetOrganisationById(id) as OkObjectResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(200);
        actual.Value.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateOrganisation
     */

    [Fact]
    public async Task CreateOrganisation_ReturnsCreated_WithSimpleOrganisationModel()
    {
        const string id = "id", name = "name";
        var organisation = new Organisation(id, name);
        var expected = _mapper.Map<SimpleOrganisationModel>(organisation);
        
        _mediator.Send(
                request: Arg.Is<CreateOrganisationCommand>(command => command.Id == id && command.Name == name), 
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(organisation);

        var sut = CreateSut();
        var actual = await sut.CreateOrganisation(new CreateOrganisationRequestModel(id, name)) as CreatedAtActionResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(201);
        actual.Value.Should().BeEquivalentTo(expected);
        
        // Check the routing
        actual.ActionName.Should().Be(nameof(OrganisationsController.GetOrganisationById));
        actual.RouteValues.Should().BeEquivalentTo(new Dictionary<string, string> { { "id", id } });
    }
    
    /*
     * UpdateOrganisation
     */
    
    [Fact]
    public async Task UpdateOrganisation_ReturnsNoContent()
    {
        const string id = "id", name = "name";
        var organisation = new Organisation(id, name);
        
        _mediator.Send(Arg.Any<UpdateOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(organisation);

        var sut = CreateSut();
        var actual = await sut.UpdateOrganisation(id, new UpdateOrganisationRequestModel(name)) as NoContentResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(204);
        
        await _mediator.Received(1).Send(
            request: Arg.Is<UpdateOrganisationCommand>(command => command.Id == id && command.Name == name),
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
        => new(_mapper, _mediator);
}