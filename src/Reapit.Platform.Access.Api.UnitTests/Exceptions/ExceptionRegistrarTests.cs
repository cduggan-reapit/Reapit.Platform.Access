using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Common.Interfaces;
using Reapit.Platform.Access.Api.Exceptions;

namespace Reapit.Platform.Access.Api.UnitTests.Exceptions;

public class ExceptionRegistrarTests
{
    private readonly IApplicationBuilder _applicationBuilder = Substitute.For<IApplicationBuilder>();
    private readonly IProblemDetailsFactory _problemDetailsFactory = Substitute.For<IProblemDetailsFactory>();
    
    [Fact]
    public void RegisterExceptions_DoesNotRegisterExceptions_WhenFactoryNotRegistered()
    {
        _applicationBuilder.ApplicationServices.GetService(typeof(IProblemDetailsFactory))
            .Returns(null);

        _applicationBuilder.RegisterExceptions();

        _problemDetailsFactory.DidNotReceive()
            .RegisterFactoryMethod<ValidationException>(Arg.Any<Func<Exception, ProblemDetails>>());
    }

    [Fact]
    public void RegisterExceptions_RegistersExceptions_WhenFactoryRegistered()
    {
        _applicationBuilder.ApplicationServices.GetService(typeof(IProblemDetailsFactory))
            .Returns(_problemDetailsFactory);

        _applicationBuilder.RegisterExceptions();

        _problemDetailsFactory.Received(1).RegisterFactoryMethod<ValidationException>(ProblemDetailFactoryImplementations.GetValidationExceptionProblemDetails);
    }
}