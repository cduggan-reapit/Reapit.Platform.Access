using AutoMapper;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Api.Controllers.Roles.V1;

/// <summary>Automapper profile for the Roles endpoints.</summary>
public class RolesProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="RolesProfile"/> class.</summary>
    public RolesProfile()
    {
        // Role => RoleModel
        CreateMap<Role, RoleModel>()
            .ForCtorParam(nameof(RoleModel.Id), ops => ops.MapFrom(entity => entity.Id))
            .ForCtorParam(nameof(RoleModel.Name), ops => ops.MapFrom(entity => entity.Name))
            .ForCtorParam(nameof(RoleModel.DateCreated), ops => ops.MapFrom(entity => DateTime.SpecifyKind(entity.DateCreated, DateTimeKind.Utc)))
            .ForCtorParam(nameof(RoleModel.DateModified), ops => ops.MapFrom(entity => DateTime.SpecifyKind(entity.DateModified, DateTimeKind.Utc)));
        
        // IEnumerable<Role> => ResultPage<RoleModel>
        CreateMap<IEnumerable<Role>, ResultPage<RoleModel>>()
            .ForCtorParam(nameof(ResultPage<RoleModel>.Data), ops => ops.MapFrom(collection => collection))
            .ForCtorParam(nameof(ResultPage<RoleModel>.Count), ops => ops.MapFrom(collection => collection.Count()))
            .ForCtorParam(nameof(ResultPage<RoleModel>.Cursor), ops => ops.MapFrom(collection => GetCursor(ref collection)));
    }

    /// <summary>Gets the maximum cursor value from a collection of objects.</summary>
    /// <param name="set">The collection.</param>
    /// <returns>The maximum Cursor value from the collection if it contains any items; otherwise zero.</returns>
    private static long GetCursor(ref IEnumerable<Role> set)
    {
        var list = set.ToList();
        return list.Any()
            ? list.Max(item => item.Cursor)
            : 0;
    }
}