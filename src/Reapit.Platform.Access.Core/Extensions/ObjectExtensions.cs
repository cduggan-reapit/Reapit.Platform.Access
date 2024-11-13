using System.Text.Json;

namespace Reapit.Platform.Access.Core.Extensions;

/// <summary>Extension methods for the <see cref="object"/> type.</summary>
public static class ObjectExtensions
{
    /// <summary>Serialize an object to a JSON string.</summary>
    /// <param name="input">The object to serialize.</param>
    public static string ToJson(this object? input) 
        => JsonSerializer.Serialize(input ?? new {});
}