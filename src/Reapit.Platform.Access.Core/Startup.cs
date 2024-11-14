using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Reapit.Platform.Access.Core;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddCoreServices(this WebApplicationBuilder builder)
    {
        // These can't reference static classes (like Startup) so just needs to point at any class in this assembly
        builder.Services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssemblyContaining<UseCases.Users.UpdateUser.UpdateUserCommandValidator>());

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(UseCases.Users.UpdateUser.UpdateUserCommandValidator));
        
        return builder;
    }
}
