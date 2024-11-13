using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Shared;

namespace Reapit.Platform.Access.Api.Controllers.Internal;

/// <summary>Base controller class for API controllers which are intended for internal system use only.</summary>
[Route("/api/internal/[controller]")]
public abstract class InternalApiController : ReapitApiController
{
}