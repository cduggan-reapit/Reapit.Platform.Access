using Microsoft.AspNetCore.Mvc;

namespace Reapit.Platform.Access.Api.Controllers.Shared;

/// <summary>
/// Base controller class for Reapit API controllers.
/// </summary>
[ApiController]
[Route("/api/[controller]")]
public abstract class ReapitApiController : ControllerBase
{
}