using Reapit.Platform.Access.Core.Exceptions;

namespace Reapit.Platform.Access.Core.UnitTests.Exceptions;

public class GroupMembershipExceptionTests
{
     /*
     * Ctor
     */

    [Fact]
    public void Ctor_InitializesException_WithNoParameters()
    {
        var sut = new GroupMembershipException();
        sut.Should().NotBeNull();
    }
    
    [Fact]
    public void Ctor_InitializesException_WithMessage()
    {
        const string message = nameof(Ctor_InitializesException_WithMessage);
        var sut = new GroupMembershipException(message);
        sut.Message.Should().Be(message);
    }
    
    [Fact]
    public void Ctor_InitializesException_WithMessage_AndInnerException()
    {
        const string message = nameof(Ctor_InitializesException_WithMessage_AndInnerException);
        var innerException = new ArgumentNullException(nameof(Ctor_InitializesException_WithMessage_AndInnerException), "message");
        var sut = new GroupMembershipException(message, innerException);
        sut.Message.Should().Be(message);
        sut.InnerException.Should().BeEquivalentTo(innerException);
    }
    
    /*
     * Pre-defined exception: CrossOrganisationMembership
     */

    [Fact]
    public void CrossOrganisationMembership_ReturnsException_WithExpectedMessage()
    {
        const string expected = "Cannot add a user to a group belonging to an organisation of which the user is not a member.";
        var sut = GroupMembershipException.CrossOrganisationMembership();
        sut.Message.Should().Be(expected);
    }
    
    /*
     * CreateProblemDetails
     */
    
    [Fact]
    public void CreateProblemDetails_ShouldThrow_WhenExceptionTypeIncorrect()
    {
        var action = () => GroupMembershipException.CreateProblemDetails(new Exception());
        action.Should().Throw<ProblemDetailsFactoryException>();
    }
    
    [Fact]
    public void CreateProblemDetails_WithExpectedProperty_ForGroupMembershipException()
    {
        const string exceptionMessage = "test-exception";
        var exception = new GroupMembershipException(exceptionMessage);
        var actual = GroupMembershipException.CreateProblemDetails(exception);
        
        actual.Type.Should().EndWith(GroupMembershipException.ProblemDetailsType);
        actual.Title.Should().Be(GroupMembershipException.ProblemDetailsTitle);
        actual.Status.Should().Be(GroupMembershipException.ProblemDetailsStatusCode);
        actual.Detail.Should().BeEquivalentTo(exceptionMessage);
    }
}