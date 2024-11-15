using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Examples;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared.Examples;
using Reapit.Platform.Access.Core.UseCases.Organisations.AddOrganisationMember;
using Reapit.Platform.Access.Core.UseCases.Organisations.DeleteOrganisation;
using Reapit.Platform.Access.Core.UseCases.Organisations.RemoveOrganisationMember;
using Reapit.Platform.Access.Core.UseCases.Organisations.SynchroniseOrganisation;
using Reapit.Platform.ApiVersioning.Attributes;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1;

/// <summary>Endpoints for interacting with organisations.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType(typeof(ProblemDetails), 400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class OrganisationsController(ISender mediator) : InternalApiController
{
    #region Synchronise Organisations
    
    /// <summary>Upsert an organisation.</summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    /// <param name="model">Definition of the properties to update.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    [SwaggerRequestExample(typeof(SynchroniseOrganisationRequestModel), typeof(UpdateOrganisationRequestModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> SynchroniseOrganisation([FromRoute] string id, [FromBody] SynchroniseOrganisationRequestModel model)
    {
        var request = new SynchroniseOrganisationCommand(id, model.Name);
        _ = await mediator.Send(request);
        return NoContent();
    }
    
    /// <summary>Delete an organisation.</summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteOrganisation([FromRoute] string id)
    {
        var request = new DeleteOrganisationCommand(id);
        _ = await mediator.Send(request);
        return NoContent();
    }
    
    #endregion
    
    #region Synchronise Organisation Membership
    
    /// <summary>Add a user to an organisation.</summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpPost("{id}/members/{userId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(409, typeof(ConflictProblemDetailsExample))]
    public async Task<IActionResult> AddOrganisationMember([FromRoute] string id, [FromRoute] string userId)
    {
        var request = new AddOrganisationMemberCommand(id, userId);
        await mediator.Send(request);
        return NoContent();
    }
    
    /// <summary>Add a user to an organisation.</summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpDelete("{id}/members/{userId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> RemoveOrganisationMember([FromRoute] string id, [FromRoute] string userId)
    {
        var request = new RemoveOrganisationMemberCommand(id, userId);
        await mediator.Send(request);
        return NoContent();
    }
    
    #endregion
}