using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Common.Exceptions;

namespace Reapit.Platform.Access.Core.Exceptions;

/// <summary>Represents and error caused by request parameters which fail validation checks.</summary>
public class QueryStringException : Exception
{
    internal const int ProblemDetailsStatusCode = 400;
    internal const string ProblemDetailsType = "https://www.reapit.com/errors/bad-request";
    internal const string ProblemDetailsTitle = "Bad Request";

    /// <summary>The collection of validation failures represented by this exception.</summary>
    public IEnumerable<ValidationFailure> Errors { get; private init; } = new List<ValidationFailure>();
    
    /// <summary>Initializes a new instance of the <see cref="QueryStringException"/> class.</summary>
    public QueryStringException() : base()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="QueryStringException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public QueryStringException(string message) 
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="QueryStringException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public QueryStringException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    /// <summary>Gets an instance of <see cref="QueryStringException"/> representing a failed <see cref="ValidationResult"/>.</summary>
    /// <param name="result">The validation result.</param>
    public static QueryStringException ValidationFailed(ValidationResult result) 
        => new("One or more validation errors occurred.") { Errors = result.Errors };

    /// <summary>Get an instance of <see cref="ProblemDetails"/> representing an Exception of type <see cref="QueryStringException"/>.</summary>
    /// <param name="exception">The exception.</param>
    /// <exception cref="Exception">the exception is not an instance of ValidationException.</exception>
    public static ProblemDetails CreateProblemDetails(Exception exception)
    {
        if (exception is not QueryStringException badRequestException)
            throw ProblemDetailsFactoryException.IncorrectExceptionMapping(exception, typeof(QueryStringException));

        var problemDetails = new ProblemDetails
        {
            Title = ProblemDetailsTitle,
            Type = ProblemDetailsType,
            Detail = badRequestException.Message,
            Status = ProblemDetailsStatusCode,
        };

        if (!badRequestException.Errors.Any())
            return problemDetails;

        problemDetails.Extensions.Add("errors", badRequestException.Errors.GroupBy(e => e.PropertyName)
            .ToDictionary(
                keySelector: group => System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(group.Key),
                elementSelector: group => group.Select(item => item.ErrorMessage)));

        return problemDetails;
    }
}