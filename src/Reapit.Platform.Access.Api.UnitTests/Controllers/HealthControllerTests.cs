using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers;

public class HealthControllerTests
{
    /*
     * HealthCheck
     */

    [Fact]
    public void HealthCheck_ReturnsNoContent_WhenCalled()
    {
        var sut = CreateSut();
        var result = sut.HealthCheck();

        var actual = result as NoContentResult;
        actual?.StatusCode.Should().Be(204);
    }
    
    /*
     * Private methods
     */

    private static HealthController CreateSut()
        => new();
}