using Reapit.Platform.Access.Core.UseCases.Groups.GetGroupById;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Groups.GetGroupById;

public class GetGroupByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRepositoryReturnsNull()
    {
        const string id = "test-id";
        _groupRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Group?>(null));

        var request = GetRequest(id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsEntity_WhenRepositoryReturnsResult()
    {
        const string id = "test-id";
        var group = new Group("name", "description", "organisation-id");
        _groupRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(group);

        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(group);
    }
    
    /*
     * Private methods
     */
    
    private GetGroupByIdQueryHandler CreateSut()
    {
        _unitOfWork.Groups.Returns(_groupRepository);
        return new GetGroupByIdQueryHandler(_unitOfWork);
    }
    
    private static GetGroupByIdQuery GetRequest(string id = "id")
        => new(id);
}