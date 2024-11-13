using Reapit.Platform.Access.Core.UseCases;
using Reapit.Platform.Access.Core.UseCases.Groups.GetGroups;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Groups.GetGroups;

public class GetGroupsQueryValidatorTests
{
    [Fact]
    public async Task Validate_Succeeds_WhenRequestValid()
    {
        var request = GetRequest(1);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    /*
     * Cursor
     */
    
    [Fact]
    public async Task Validate_Succeeds_WhenCursorLessThanZero()
    {
        var request = GetRequest(-1);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(GetGroupsQuery.Cursor), CommonValidationMessages.CursorOutOfRange);
    }
    
    /*
     * PageSize
     */
    
    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task Validate_Succeeds_WhenPageSizeOutOfRange(int pageSize)
    {
        var request = GetRequest(pageSize: pageSize);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(GetGroupsQuery.PageSize), CommonValidationMessages.PageSizeOutOfRange);
    }
    
    /*
     * Private methods
     */

    private static GetGroupsQueryValidator CreateSut() => new();

    private static GetGroupsQuery GetRequest(long? cursor = null, int pageSize = 25)
        => new(cursor, pageSize);
}