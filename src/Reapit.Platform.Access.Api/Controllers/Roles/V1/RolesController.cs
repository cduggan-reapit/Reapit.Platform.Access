using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Examples;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Api.Controllers.Shared.Examples;
using Reapit.Platform.Access.Core.UseCases.Roles.CreateRole;
using Reapit.Platform.Access.Core.UseCases.Roles.DeleteRole;
using Reapit.Platform.Access.Core.UseCases.Roles.GetRoleById;
using Reapit.Platform.Access.Core.UseCases.Roles.GetRoles;
using Reapit.Platform.Access.Core.UseCases.Roles.PatchRole;
using Reapit.Platform.Access.Core.UseCases.UserRoles.AddUserRole;
using Reapit.Platform.Access.Core.UseCases.UserRoles.RemoveUserRole;
using Reapit.Platform.ApiVersioning.Attributes;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Roles.V1;

/// <summary>Endpoints for interacting with user roles.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType<ProblemDetails>(400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class RolesController(IMapper mapper, ISender mediator) : ReapitApiController
{
    /// <summary>Get a page of roles with optional filters.</summary>
    [HttpGet]
    [ProducesResponseType<ResultPage<RoleModel>>(200)]
    [ProducesResponseType<ProblemDetails>(400)]
    [SwaggerResponseExample(200, typeof(RoleModelResultPageExample))]
    [SwaggerResponseExample(400, typeof(QueryStringProblemDetailsExample))]
    public async Task<IActionResult> GetRoles([FromQuery] GetRolesRequestModel model)
    {
        var request = mapper.Map<GetRolesQuery>(model);
        var result = await mediator.Send(request);
        return Ok(mapper.Map<ResultPage<RoleModel>>(result));
    }

    /// <summary>Get an individual role.</summary>
    /// <param name="id">The unique identifier of the role.</param>
    [HttpGet("{id}")]
    [ProducesResponseType<RoleModel>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(200, typeof(RoleModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetRoleById([FromRoute] string id)
    {
        var request = new GetRoleByIdQuery(id);
        var result = await mediator.Send(request);
        return Ok(mapper.Map<RoleModel>(result));
    }
    
    /// <summary>Create a new role.</summary>
    /// <param name="model">Definition of the role to create.</param>
    [HttpPost]
    [ProducesResponseType<RoleModel>(201)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(CreateRoleRequestModel), typeof(CreateRoleRequestModelExample))]
    [SwaggerResponseExample(201, typeof(RoleModelExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestModel model)
    {
        var request = new CreateRoleCommand(model.Name, model.Description);
        var result = await mediator.Send(request);
        var roleModel = mapper.Map<RoleModel>(result);
        return CreatedAtAction(nameof(GetRoleById), new { id = roleModel.Id }, roleModel);
    }

    /// <summary>Update a role.</summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <param name="model">Definition of the properties to update.</param>
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(PatchRoleRequestModel), typeof(PatchRoleRequestModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> PatchRole([FromRoute] string id, [FromBody] PatchRoleRequestModel model)
    {
        var request = new PatchRoleCommand(id, model.Name, model.Description);
        _ = await mediator.Send(request);
        return NoContent();
    }

    /// <summary>Delete a role.</summary>
    /// <param name="id">The unique identifier of the role.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteRole([FromRoute] string id)
    {
        var request = new SoftDeleteRoleCommand(id);
        _ = await mediator.Send(request);
        return NoContent();
    }
    
    /// <summary>Add a user to a role.</summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpPost("{id}/users/{userId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(409)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(409, typeof(ConflictProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> AddUser([FromRoute] string id, [FromRoute] string userId)
    {
        var request = new AddUserRoleCommand(RoleId: id, UserId: userId);
        await mediator.Send(request);
        return NoContent();
    }
    
    /// <summary>Remove a user from a role.</summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpDelete("{id}/users/{userId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> RemoveUser([FromRoute] string id, [FromRoute] string userId)
    {
        var request = new RemoveUserRoleCommand(RoleId: id, UserId: userId);
        await mediator.Send(request);
        return NoContent();
    }
}