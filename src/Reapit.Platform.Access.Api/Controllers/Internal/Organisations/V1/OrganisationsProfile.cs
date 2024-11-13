using AutoMapper;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1;

/// <summary>AutoMapper profile used by organisations endpoints.</summary>
public class OrganisationsProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="OrganisationsProfile"/> class.</summary>
    public OrganisationsProfile()
    {
        CreateMap<Organisation, SimpleOrganisationModel>()
            .ForCtorParam(nameof(SimpleOrganisationModel.Id), ops => ops.MapFrom(src => src.Id))
            .ForCtorParam(nameof(SimpleOrganisationModel.Name), ops => ops.MapFrom(src => src.Name))
            .ForCtorParam(nameof(SimpleOrganisationModel.DateLastSynchronised), ops => ops.MapFrom(src => src.DateLastSynchronised.UtcDateTime));
    }
}