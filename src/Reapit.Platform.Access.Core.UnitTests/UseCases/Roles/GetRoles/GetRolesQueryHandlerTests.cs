using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.UseCases.Roles.GetRoles;
using Reapit.Platform.Access.Data.Repositories;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Roles.GetRoles;

public class GetRolesQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly IValidator<GetRolesQuery> _validator = Substitute.For<IValidator<GetRolesQuery>>();
    private readonly FakeLogger<GetRolesQueryHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsQueryStringException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<GetRolesQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<QueryValidationException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntities_WhenValidationSucceeds()
    {
        _validator.ValidateAsync(Arg.Any<GetRolesQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var request = GetRequest(
            createdFrom: DateTime.UnixEpoch.AddDays(1),
            createdTo: DateTime.UnixEpoch.AddDays(2),
            modifiedFrom: DateTime.UnixEpoch.AddDays(3),
            modifiedTo: DateTime.UnixEpoch.AddDays(4));

        // Two-birds, one stone.  Make sure the query parameters are passed correctly by only returning the value if they are.
        var pagination = new PaginationFilter(request.Cursor, request.PageSize);
        var timestamps = new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo);
        
        var groups = new[] { new Role("name", "description") };
        _roleRepository.GetRolesAsync(
                request.UserId, 
                request.Name, 
                request.Description, 
                pagination,
                timestamps,
                Arg.Any<CancellationToken>())
            .Returns(groups);

        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(groups);
    }
    
    /*
     * Private methods
     */
    
    private GetRolesQueryHandler CreateSut()
    {
        _unitOfWork.Roles.Returns(_roleRepository);
        return new GetRolesQueryHandler(_unitOfWork, _validator);
    }
    
    private static GetRolesQuery GetRequest(
        long? cursor = 1731533393578244L, 
        int pageSize = 25,
        string? userId = "user-id",
        string? name = "name",
        string? description = "description",
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        DateTime? modifiedFrom = null,
        DateTime? modifiedTo = null)
        => new(cursor, pageSize, userId, name, description, createdFrom, createdTo, modifiedFrom, modifiedTo);
}