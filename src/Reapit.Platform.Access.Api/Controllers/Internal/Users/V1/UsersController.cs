using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Internal.Users.V1.Examples;
using Reapit.Platform.Access.Api.Controllers.Internal.Users.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared.Examples;
using Reapit.Platform.Access.Core.UseCases.Users.DeleteUser;
using Reapit.Platform.Access.Core.UseCases.Users.SynchroniseUser;
using Reapit.Platform.ApiVersioning.Attributes;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Internal.Users.V1;

/// <summary>Endpoints for interacting with users.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType(typeof(ProblemDetails), 400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class UsersController(ISender mediator) : InternalApiController
{
    /// <summary>Delete a user from the user collection.</summary>
    /// <param name="id">the unique identifier of the user.</param>
    /// <remarks>Not advertised in Swagger.</remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var command = new DeleteUserCommand(id);
        _ = await mediator.Send(command);
        return NoContent();
    }
    
    /// <summary>Upsert a user in the user collection.</summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="model">The notification user model.</param>
    /// <remarks>Not advertised in Swagger.</remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    [SwaggerRequestExample(typeof(SynchroniseUserRequestModel), typeof(UpdateUserRequestModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> SynchroniseUser(string id, SynchroniseUserRequestModel model)
    {
        var command = new SynchroniseUserCommand(id, model.Name, model.Email);
        _ = await mediator.Send(command);
        return NoContent();
    }
}