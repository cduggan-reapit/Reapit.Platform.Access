using AutoMapper;
using Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1.Models;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1;

/// <summary>AutoMapper profile for user-related objects.</summary>
public class UsersProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="UsersProfile"/> class.</summary>
    public UsersProfile()
    {
        // User => UserModel
        CreateMap<User, SimpleUserModel>()
            .ForCtorParam(nameof(SimpleUserModel.Id), ops => ops.MapFrom(src => src.Id))
            .ForCtorParam(nameof(SimpleUserModel.Name), ops => ops.MapFrom(src => src.Name))
            .ForCtorParam(nameof(SimpleUserModel.Email), ops => ops.MapFrom(src => src.Email))
            .ForCtorParam(nameof(SimpleUserModel.DateLastSynchronised), ops => ops.MapFrom(src => src.DateLastSynchronised.UtcDateTime));

        
      
    }
}