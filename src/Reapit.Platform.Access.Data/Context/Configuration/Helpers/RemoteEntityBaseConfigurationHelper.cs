using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Domain.Entities.Abstract;

namespace Reapit.Platform.Access.Data.Context.Configuration.Helpers;

public static class RemoteEntityBaseConfigurationHelper
{
    public static EntityTypeBuilder<T> ConfigureRemoteEntityBase<T>(this EntityTypeBuilder<T> builder)
        where T : RemoteEntityBase
    {
        builder.Property(entity => entity.DateLastSynchronised).HasColumnName("last_sync");
        return builder;
    }
}