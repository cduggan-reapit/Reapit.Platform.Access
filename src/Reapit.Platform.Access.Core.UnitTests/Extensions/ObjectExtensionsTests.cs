using System.Text.Json;
using Reapit.Platform.Access.Core.Extensions;

namespace Reapit.Platform.Access.Core.UnitTests.Extensions;

public class ObjectExtensionsTests
{
    /*
     * ToJson
     */

    [Fact]
    public void ToJson_ReturnsNull_WhenInputNull()
    {
        var input = null as object;
        var expected = JsonSerializer.Serialize(new { });
        var actual = input.ToJson();
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ToJson_ReturnsSerializedString_WhenInputNotNull()
    {
        var input = new { Property = "Value" };
        var expected = JsonSerializer.Serialize(input);
        var actual = input.ToJson();
        actual.Should().BeEquivalentTo(expected);
    }
}