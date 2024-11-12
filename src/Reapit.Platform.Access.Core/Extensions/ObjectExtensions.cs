using System.Text.Json;

namespace Reapit.Platform.Access.Core.Extensions;

/// <summary>Extension methods for the <see cref="object"/> type.</summary>
public static class ObjectExtensions
{
    /// <summary>Serialize an object to a JSON string.</summary>
    /// <param name="input">The object to serialize.</param>
    public static string ToJson(this object? input) 
        => JsonSerializer.Serialize(input ?? new {});
    
    /// <summary>Deserialize a <typeparamref name="T"/> from a JSON string.</summary>
    /// <param name="input">The string to deserialize.</param>
    /// <typeparam name="T">The target type .</typeparam>
    /// <remarks>
    /// We use the not-null assertion for the deserialize result.  The documentation for JsonSerializer.Deserialize
    /// suggests that a null response here is not possible. See <a href="https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer.deserialize?view=net-8.0#system-text-json-jsonserializer-deserialize(system-text-json-jsondocument-system-type-system-text-json-jsonserializeroptions)">
    /// Microsoft Docs</a> for more information. 
    /// </remarks>
    public static T FromJson<T>(this string input) 
        => JsonSerializer.Deserialize<T>(input)!;
}