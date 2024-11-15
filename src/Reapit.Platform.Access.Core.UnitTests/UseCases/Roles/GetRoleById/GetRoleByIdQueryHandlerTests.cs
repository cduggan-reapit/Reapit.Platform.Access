using Reapit.Platform.Access.Core.UseCases.Roles.GetRoleById;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Roles.GetRoleById;

public class GetRoleByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRepositoryReturnsNull()
    {
        const string id = "test-id";
        _roleRepository.GetRoleByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Role?>(null));

        var request = GetRequest(id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsEntity_WhenRepositoryReturnsResult()
    {
        const string id = "test-id";
        var role = new Role("name", "description");
        _roleRepository.GetRoleByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(role);

        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(role);
    }
    
    /*
     * Private methods
     */
    
    private GetRoleByIdQueryHandler CreateSut()
    {
        _unitOfWork.Roles.Returns(_roleRepository);
        return new GetRoleByIdQueryHandler(_unitOfWork);
    }
    
    private static GetRoleByIdQuery GetRequest(string id = "id")
        => new(id);
}