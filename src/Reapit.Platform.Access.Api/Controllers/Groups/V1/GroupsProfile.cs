using AutoMapper;
using Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Core.UseCases.Groups.GetGroups;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Api.Controllers.Groups.V1;

/// <summary>AutoMapper profile used by Groups endpoints.</summary>
public class GroupsProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="GroupsProfile"/> class.</summary>
    public GroupsProfile()
    {
        // Group => GroupModel
        CreateMap<Group, GroupModel>()
            .ForCtorParam(nameof(GroupModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(GroupModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(GroupModel.OrganisationId), ops => ops.MapFrom(entity => entity.OrganisationId))
            .ForCtorParam(nameof(GroupModel.DateCreated), ops => ops.MapFrom(entity => DateTime.SpecifyKind(entity.DateCreated, DateTimeKind.Utc)))
            .ForCtorParam(nameof(GroupModel.DateModified), ops => ops.MapFrom(entity => DateTime.SpecifyKind(entity.DateModified, DateTimeKind.Utc)));
        
        // GetGroupsRequestModel => GetGroupsQuery
        CreateMap<GetGroupsRequestModel, GetGroupsQuery>()
            .ForCtorParam(nameof(GetGroupsQuery.Cursor), ops => ops.MapFrom(request => request.Cursor))
            .ForCtorParam(nameof(GetGroupsQuery.PageSize), ops => ops.MapFrom(request => request.PageSize))
            .ForCtorParam(nameof(GetGroupsQuery.UserId), ops => ops.MapFrom(request => request.UserId))
            .ForCtorParam(nameof(GetGroupsQuery.OrganisationId), ops => ops.MapFrom(request => request.OrganisationId))
            .ForCtorParam(nameof(GetGroupsQuery.Name), ops => ops.MapFrom(request => request.Name))
            .ForCtorParam(nameof(GetGroupsQuery.Description), ops => ops.MapFrom(request => request.Description))
            .ForCtorParam(nameof(GetGroupsQuery.CreatedFrom), ops => ops.MapFrom(request => request.CreatedFrom))
            .ForCtorParam(nameof(GetGroupsQuery.CreatedTo), ops => ops.MapFrom(request => request.CreatedTo))
            .ForCtorParam(nameof(GetGroupsQuery.ModifiedFrom), ops => ops.MapFrom(request => request.ModifiedFrom))
            .ForCtorParam(nameof(GetGroupsQuery.ModifiedTo), ops => ops.MapFrom(request => request.ModifiedTo));
        
        // IEnumerable<Group> => PagedResult<GroupModel>
        CreateMap<IEnumerable<Group>, ResultPage<GroupModel>>()
            .ForCtorParam(nameof(ResultPage<GroupModel>.Data), ops => ops.MapFrom(collection => collection))
            .ForCtorParam(nameof(ResultPage<GroupModel>.Count), ops => ops.MapFrom(collection => collection.Count()))
            .ForCtorParam(nameof(ResultPage<GroupModel>.Cursor), ops => ops.MapFrom(collection => GetCursor(ref collection)));
    }

    /// <summary>Gets the maximum cursor value from a collection of objects.</summary>
    /// <param name="set">The collection.</param>
    /// <returns>The maximum Cursor value from the collection if it contains any items; otherwise zero.</returns>
    private static long GetCursor(ref IEnumerable<Group> set)
    {
        var list = set.ToList();
        return list.Any()
            ? list.Max(item => item.Cursor)
            : 0;
    }
}