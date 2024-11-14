using Microsoft.AspNetCore.Mvc;

namespace Reapit.Platform.Access.Core.Exceptions;

/// <summary>Represents an error arising from a group membership.</summary>
public class GroupMembershipException : Exception
{
    internal const int ProblemDetailsStatusCode = 400;
    internal const string ProblemDetailsType = "https://www.reapit.com/errors/bad-request";
    internal const string ProblemDetailsTitle = "Bad Request";
    
    /// <summary>Initializes a new instance of the <see cref="GroupMembershipException"/> class.</summary>
    public GroupMembershipException() : base()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="GroupMembershipException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public GroupMembershipException(string message) 
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="GroupMembershipException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GroupMembershipException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
    
    /// <summary>Get an instance of <see cref="GroupMembershipException"/> representing an attempt to add a user to an invalid organisation.</summary>
    public static GroupMembershipException CrossOrganisationMembership()
        => new("Cannot add a user to a group belonging to an organisation of which the user is not a member.");
    
    /// <summary>Get an instance of <see cref="ProblemDetails"/> representing an Exception of type <see cref="ConflictException"/>.</summary>
    /// <param name="exception">The exception.</param>
    /// <exception cref="Exception">the exception is not an instance of ConflictException.</exception>
    public static ProblemDetails CreateProblemDetails(Exception exception)
    {
        if (exception is not GroupMembershipException _)
            throw ProblemDetailsFactoryException.IncorrectExceptionMapping(exception, typeof(GroupMembershipException));

        return new ProblemDetails
        {
            Title = ProblemDetailsTitle,
            Type = ProblemDetailsType,
            Status = ProblemDetailsStatusCode,
            Detail = exception.Message
        };
    }
}