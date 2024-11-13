using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.UseCases.Groups.GetGroups;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Groups.GetGroups;

public class GetGroupsQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();
    private readonly IValidator<GetGroupsQuery> _validator = Substitute.For<IValidator<GetGroupsQuery>>();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsQueryStringException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<GetGroupsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<QueryValidationException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntities_WhenValidationSucceeds()
    {
        _validator.ValidateAsync(Arg.Any<GetGroupsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var request = GetRequest(
            createdFrom: DateTime.UnixEpoch.AddDays(1),
            createdTo: DateTime.UnixEpoch.AddDays(2),
            modifiedFrom: DateTime.UnixEpoch.AddDays(3),
            modifiedTo: DateTime.UnixEpoch.AddDays(4));

        // Two-birds, one stone.  Make sure the query parameters are passed correctly by only returning the value if they are.
        var groups = new[] { new Group("name", "description", "organisationId") };
        _groupRepository.GetGroupsAsync(
                request.Cursor, 
                request.PageSize, 
                request.UserId, 
                request.OrganisationId,
                request.Name, 
                request.Description, 
                request.CreatedFrom, 
                request.CreatedTo, 
                request.ModifiedFrom,
                request.ModifiedTo, 
                Arg.Any<CancellationToken>())
            .Returns(groups);

        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(groups);
    }
    
    /*
     * Private methods
     */
    
    private GetGroupsQueryHandler CreateSut()
    {
        _unitOfWork.Groups.Returns(_groupRepository);
        return new GetGroupsQueryHandler(_unitOfWork, _validator);
    }
    
    private static GetGroupsQuery GetRequest(
        long? cursor = 1731533393578244L, 
        int pageSize = 25,
        string? userId = "user-id",
        string? organisationId = "organisation-id",
        string? name = "name",
        string? description = "description",
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        DateTime? modifiedFrom = null,
        DateTime? modifiedTo = null)
        => new(cursor, pageSize, userId, organisationId, name, description, createdFrom, createdTo, modifiedFrom, modifiedTo);
}