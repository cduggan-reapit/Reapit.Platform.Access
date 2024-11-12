using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Examples;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Api.Controllers.Shared.Examples;
using Reapit.Platform.Access.Core.UseCases.Organisations.AddOrganisationMember;
using Reapit.Platform.Access.Core.UseCases.Organisations.CreateOrganisation;
using Reapit.Platform.Access.Core.UseCases.Organisations.DeleteOrganisation;
using Reapit.Platform.Access.Core.UseCases.Organisations.GetOrganisationById;
using Reapit.Platform.Access.Core.UseCases.Organisations.RemoveOrganisationMember;
using Reapit.Platform.Access.Core.UseCases.Organisations.UpdateOrganisation;
using Reapit.Platform.ApiVersioning.Attributes;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1;

/// <summary>Endpoints for interacting with organisations.</summary>
[Route("/internal/[controller]")]
[IntroducedInVersion(1, 0)]
[ProducesResponseType(typeof(ProblemDetails), 400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class OrganisationsController : ReapitApiController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    
    /// <summary>Initializes a new instance of the <see cref="OrganisationsController"/> class.</summary>
    /// <param name="mediator">The mediator service.</param>
    /// <param name="mapper">The automapper service.</param>
    public OrganisationsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>Get an individual organisation.</summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SimpleOrganisationModel), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(200, typeof(OrganisationModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetOrganisationById([FromRoute] string id)
    {
        var request = new GetOrganisationByIdQuery(id);
        var organisation = await _mediator.Send(request);
        return Ok(_mapper.Map<SimpleOrganisationModel>(organisation));
    }
    
    /// <summary>Create a new organisation.</summary>
    /// <param name="model">Definition of the organisation to create.</param>
    [HttpPost]
    [ProducesResponseType(typeof(SimpleOrganisationModel), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    [SwaggerRequestExample(typeof(CreateOrganisationRequestModel), typeof(CreateOrganisationRequestModelExample))]
    [SwaggerResponseExample(200, typeof(OrganisationModelExample))]
    [SwaggerResponseExample(409, typeof(ConflictProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> CreateOrganisation([FromBody] CreateOrganisationRequestModel model)
    {
        var request = new CreateOrganisationCommand(model.Id, model.Name);
        var organisation = await _mediator.Send(request);
        return CreatedAtAction(
            actionName: nameof(GetOrganisationById),
            routeValues: new { id = organisation.Id },
            value: _mapper.Map<SimpleOrganisationModel>(organisation));
    }
    
    /// <summary>Update an organisation.</summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    /// <param name="model">Definition of the properties to update.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    [SwaggerRequestExample(typeof(UpdateOrganisationRequestModel), typeof(UpdateOrganisationRequestModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> UpdateOrganisation([FromRoute] string id, [FromBody] UpdateOrganisationRequestModel model)
    {
        var request = new UpdateOrganisationCommand(id, model.Name);
        _ = await _mediator.Send(request);
        return NoContent();
    }
    
    /// <summary>Update an organisation.</summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteOrganisation([FromRoute] string id)
    {
        var request = new DeleteOrganisationCommand(id);
        _ = await _mediator.Send(request);
        return NoContent();
    }
    
    /// <summary>Add a user to an organisation.</summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpPost("{id}/members/{userId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> AddOrganisationMember([FromRoute] string id, [FromRoute] string userId)
    {
        var request = new AddOrganisationMemberCommand(id, userId);
        var organisation = await _mediator.Send(request);
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
        var organisation = await _mediator.Send(request);
        return NoContent();
    }
}