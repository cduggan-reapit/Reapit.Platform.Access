using Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Groups.V1.Examples;

/// <summary>Example provider for the <see cref="CreateGroupRequestModel"/> type.</summary>
public class CreateGroupRequestModelExample : IExamplesProvider<CreateGroupRequestModel>
{
    /// <inheritdoc />
    public CreateGroupRequestModel GetExamples()
        => new(
            Name: "Swindon Office Users", 
            Description: "Users with access to offices in Swindon and north-east Wiltshire", 
            OrganisationId: "5c9b4a946be842f69163958b6d97e233");
}