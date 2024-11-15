using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Core.UnitTests.Extensions;

public class EntityBaseExtensions
{
    /*
     * GetMaximumCursor
     */

    [Fact]
    public void GetMaximumCursor_ReturnsZero_WhenProvidedAnEmptyCollection()
    {
        var collection = Array.Empty<Product>();
        var actual = collection.GetMaximumCursor();
        actual.Should().Be(0);
    }
    
    [Fact]
    public void GetMaximumCursor_ReturnsMaximumValue_WhenCollectionPopulated()
    {
        using var _ = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch.AddMicroseconds(500));
        
        // cursor should be X for unix epoch, +500 for the test
        const long expected = 500;

        var collection = new[] { new Product("name") };
        var actual = collection.GetMaximumCursor();
        actual.Should().Be(expected);
    }
}