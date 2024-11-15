using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Examples;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Api.Controllers.Shared.Examples;
using Reapit.Platform.Access.Core.UseCases.Roles.GetRoleById;
using Reapit.Platform.Access.Core.UseCases.Roles.GetRoles;
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

    /// <summary>Get an individual role with optional filters.</summary>
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
}