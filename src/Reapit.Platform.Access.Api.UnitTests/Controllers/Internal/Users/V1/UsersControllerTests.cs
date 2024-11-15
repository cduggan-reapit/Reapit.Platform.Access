using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Internal.Users.V1;
using Reapit.Platform.Access.Api.Controllers.Internal.Users.V1.Models;
using Reapit.Platform.Access.Core.UseCases.Users.DeleteUser;
using Reapit.Platform.Access.Core.UseCases.Users.SynchroniseUser;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers.Internal.Users.V1;

// Most tests for controllers will be integration tests, this is _really_ just to check that we're mapping responses and
// building mediator request objects correctly.
public class UsersControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    
    /*
     * SynchroniseUser
     */
    
    [Fact]
    public async Task SynchroniseUser_ReturnsNoContent()
    {
        const string id = "id", name = "name", email = "email";
        var user = new User(id, name, email);
        
        _mediator.Send(Arg.Any<SynchroniseUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var sut = CreateSut();
        var actual = await sut.SynchroniseUser(id, new SynchroniseUserRequestModel(name, email)) as NoContentResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(204);
        
        await _mediator.Received(1).Send(
            request: Arg.Is<SynchroniseUserCommand>(command => command.Id == id && command.Name == name && command.Email == email),
            cancellationToken: Arg.Any<CancellationToken>());
    }
    
    /*
     * DeleteUser
     */
    
    [Fact]
    public async Task DeleteUser_ReturnsNoContent()
    {
        const string id = "id";
        var user = new User(id, "name", "email");
        
        _mediator.Send(Arg.Any<DeleteUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var sut = CreateSut();
        var actual = await sut.DeleteUser(id) as NoContentResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(204);

        await _mediator.Received(1).Send(
            request: Arg.Is<DeleteUserCommand>(command => command.Id == id),
            cancellationToken: Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private UsersController CreateSut()
        => new(_mediator);
}