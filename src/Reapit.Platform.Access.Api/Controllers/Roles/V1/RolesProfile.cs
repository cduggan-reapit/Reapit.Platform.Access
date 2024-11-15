using AutoMapper;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Core.UseCases.Roles.GetRoles;
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
            .ForCtorParam(nameof(ResultPage<RoleModel>.Cursor), ops => ops.MapFrom(collection => collection.GetMaximumCursor()));
        
        // GetRolesRequestModel => GetRolesQuery
        CreateMap<GetRolesRequestModel, GetRolesQuery>()
            .ForCtorParam(nameof(GetRolesQuery.Cursor), ops => ops.MapFrom(request => request.Cursor))
            .ForCtorParam(nameof(GetRolesQuery.PageSize), ops => ops.MapFrom(request => request.PageSize))
            .ForCtorParam(nameof(GetRolesQuery.UserId), ops => ops.MapFrom(request => request.UserId))
            .ForCtorParam(nameof(GetRolesQuery.Name), ops => ops.MapFrom(request => request.Name))
            .ForCtorParam(nameof(GetRolesQuery.Description), ops => ops.MapFrom(request => request.Description))
            .ForCtorParam(nameof(GetRolesQuery.CreatedFrom), ops => ops.MapFrom(request => request.CreatedFrom))
            .ForCtorParam(nameof(GetRolesQuery.CreatedTo), ops => ops.MapFrom(request => request.CreatedTo))
            .ForCtorParam(nameof(GetRolesQuery.ModifiedFrom), ops => ops.MapFrom(request => request.ModifiedFrom))
            .ForCtorParam(nameof(GetRolesQuery.ModifiedTo), ops => ops.MapFrom(request => request.ModifiedTo));
    }
}