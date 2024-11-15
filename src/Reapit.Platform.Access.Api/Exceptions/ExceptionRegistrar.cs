using FluentValidation;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Common.Interfaces;

namespace Reapit.Platform.Access.Api.Exceptions;

/// <summary>ProblemDetail factory registrar for application exceptions.</summary>
public static class ExceptionRegistrar
{
    /// <summary>Register factory methods for exceptions defined in this project with the <see cref="IProblemDetailsFactory"/>.</summary>
    /// <param name="app">The service collection</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IApplicationBuilder RegisterExceptions(this IApplicationBuilder app)
    {
        var factory = app.ApplicationServices.GetService<IProblemDetailsFactory>();
        
        if (factory is null)
            return app;
        
        // Validation errors arising from write endpoints (422)
        factory.RegisterFactoryMethod<ValidationException>(ProblemDetailFactoryImplementations.GetValidationExceptionProblemDetails);
        
        // Validation errors arising from read endpoints (400)
        factory.RegisterFactoryMethod<QueryValidationException>(QueryValidationException.CreateProblemDetails);
        
        // Conflict errors - thing already exists (409) 
        factory.RegisterFactoryMethod<ConflictException>(ConflictException.CreateProblemDetails);
        
        // Not sure what code this should have yet - 400 for now
        factory.RegisterFactoryMethod<GroupMembershipException>(GroupMembershipException.CreateProblemDetails);

        return app;
    }
    
    
}