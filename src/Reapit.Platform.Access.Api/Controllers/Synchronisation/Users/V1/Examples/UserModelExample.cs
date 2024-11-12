using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1.Examples;

/// <summary>Example provider for the <see cref="SimpleUserModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class UserModelExample : IExamplesProvider<SimpleUserModel>
{
    /// <inheritdoc />
    public SimpleUserModel GetExamples()
        => new(
            Id: "6cd3556deb0da54bca060b4c39479839", 
            Name: "Joseph Bloggs", 
            Email: "jbloggs@reapit.com", 
            DateLastSynchronised: new DateTime(2024, 11, 12, 10, 27, 13));
}