using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Groups.V1.Examples;
using Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Api.Controllers.Shared.Examples;
using Reapit.Platform.Access.Core.UseCases.GroupMembership.AddGroupMember;
using Reapit.Platform.Access.Core.UseCases.GroupMembership.RemoveGroupMember;
using Reapit.Platform.Access.Core.UseCases.Groups.CreateGroup;
using Reapit.Platform.Access.Core.UseCases.Groups.DeleteGroup;
using Reapit.Platform.Access.Core.UseCases.Groups.GetGroupById;
using Reapit.Platform.Access.Core.UseCases.Groups.GetGroups;
using Reapit.Platform.Access.Core.UseCases.Groups.PatchGroup;
using Reapit.Platform.ApiVersioning.Attributes;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Groups.V1;

/// <summary>Endpoints for interacting with user groups.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType<ProblemDetails>(400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class GroupsController(IMapper mapper, ISender mediator) : ReapitApiController
{
    /// <summary>Get a page of groups with optional filters.</summary>
    [HttpGet]
    [ProducesResponseType<ResultPage<GroupModel>>(200)]
    [ProducesResponseType<ProblemDetails>(400)]
    [SwaggerResponseExample(200, typeof(GroupModelResultPageExample))]
    [SwaggerResponseExample(400, typeof(QueryStringProblemDetailsExample))]
    public async Task<IActionResult> GetGroups([FromQuery] GetGroupsRequestModel model)
    {
        var request = mapper.Map<GetGroupsQuery>(model);
        var result = await mediator.Send(request);
        return Ok(mapper.Map<ResultPage<GroupModel>>(result));
    }
    
    /// <summary>Get an individual group.</summary>
    /// <param name="id">The unique identifier of the group.</param>
    [HttpGet("{id}")]
    [ProducesResponseType<GroupModel>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(200, typeof(GroupModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetGroupById([FromRoute] string id)
    {
        var request = new GetGroupByIdQuery(id);
        var result = await mediator.Send(request);
        return Ok(mapper.Map<GroupModel>(result));
    }

    /// <summary>Create a new group.</summary>
    /// <param name="model">Definition of the group to create.</param>
    [HttpPost]
    [ProducesResponseType<GroupModel>(201)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(CreateGroupRequestModel), typeof(CreateGroupRequestModelExample))]
    [SwaggerResponseExample(201, typeof(GroupModelExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequestModel model)
    {
        var request = new CreateGroupCommand(model.Name, model.Description, model.OrganisationId);
        var result = await mediator.Send(request);
        var groupModel = mapper.Map<GroupModel>(result);
        return CreatedAtAction(nameof(GetGroupById), new { id = groupModel.Id }, groupModel);
    }

    /// <summary>Update a group.</summary>
    /// <param name="id">The unique identifier of the group.</param>
    /// <param name="model">Definition of the properties to update.</param>
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(PatchGroupRequestModel), typeof(PatchGroupRequestModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> PatchGroup([FromRoute] string id, [FromBody] PatchGroupRequestModel model)
    {
        var request = new PatchGroupCommand(id, model.Name, model.Description);
        _ = await mediator.Send(request);
        return NoContent();
    }

    /// <summary>Delete a group.</summary>
    /// <param name="id">The unique identifier of the group.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteGroup([FromRoute] string id)
    {
        var request = new SoftDeleteGroupCommand(id);
        _ = await mediator.Send(request);
        return NoContent();
    }

    #region Membership

    /// <summary>Add a user to a group.</summary>
    /// <param name="id">The unique identifier of the group.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpPost("{id}/members/{userId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(409)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(409, typeof(ConflictProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> AddMember([FromRoute] string id, [FromRoute] string userId)
    {
        var request = new AddGroupMemberCommand(GroupId: id, UserId: userId);
        await mediator.Send(request);
        return NoContent();
    }
    
    /// <summary>Remove a user from a group.</summary>
    /// <param name="id">The unique identifier of the group.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpDelete("{id}/members/{userId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> RemoveMember([FromRoute] string id, [FromRoute] string userId)
    {
        var request = new RemoveGroupMemberCommand(GroupId: id, UserId: userId);
        await mediator.Send(request);
        return NoContent();
    }

    #endregion
}