using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Api.Controllers.Shared.Examples;
using Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1.Examples;
using Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1.Models;
using Reapit.Platform.Access.Core.UseCases.Users.CreateUser;
using Reapit.Platform.Access.Core.UseCases.Users.DeleteUser;
using Reapit.Platform.Access.Core.UseCases.Users.GetUserById;
using Reapit.Platform.Access.Core.UseCases.Users.UpdateUser;
using Reapit.Platform.ApiVersioning.Attributes;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1;

/// <summary>Endpoints for interacting with users.</summary>
[Route("/access/internal/[controller]")]
[IntroducedInVersion(1, 0)]
[ProducesResponseType(typeof(ProblemDetails), 400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class UsersController : ReapitApiController
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    /// <summary>Initializes a new instance of the <see cref="UsersController"/> class.</summary>
    /// <param name="mapper">The object-to-object mapping service.</param>
    /// <param name="mediator">The in-process messaging service.</param>
    public UsersController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    /// <summary>Get an individual user record.</summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>Details of the user. For user group information, please use the OrganisationUser endpoints.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SimpleUserModel), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetUserById(string id)
    {
        var query = new GetUserByIdQuery(id);
        var user = await _mediator.Send(query);
        return Ok(_mapper.Map<SimpleUserModel>(user));
    }
    
    /// <summary>Add a user to the users collection.</summary>
    /// <param name="model">The notification user model.</param>
    /// <remarks>Not advertised in Swagger.</remarks>
    [HttpPost]
    [ProducesResponseType(typeof(SimpleUserModel), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    [SwaggerRequestExample(typeof(CreateUserRequestModel), typeof(CreateUserRequestModelExample))]
    [SwaggerResponseExample(204, typeof(UserModelExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> CreateUser(CreateUserRequestModel model)
    {
        var command = new CreateUserCommand(model.Id, model.Name, model.Email);
        var user = await _mediator.Send(command);
        var responseModel = _mapper.Map<SimpleUserModel>(user);
        return CreatedAtAction(nameof(GetUserById), new { id = responseModel.Id }, responseModel);
    }
    
    /// <summary>Update a user in the users collection.</summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="model">The notification user model.</param>
    /// <remarks>Not advertised in Swagger.</remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    [SwaggerRequestExample(typeof(UpdateUserRequestModel), typeof(UpdateUserRequestModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationFailedProblemDetailsExample))]
    public async Task<IActionResult> UpdateUser(string id, UpdateUserRequestModel model)
    {
        var command = new UpdateUserCommand(id, model.Name, model.Email);
        _ = await _mediator.Send(command);
        return NoContent();
    }
    
    /// <summary>Delete a user from the users collection.</summary>
    /// <param name="id">the unique identifier of the user.</param>
    /// <remarks>Not advertised in Swagger.</remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var command = new DeleteUserCommand(id);
        _ = await _mediator.Send(command);
        return NoContent();
    }
}