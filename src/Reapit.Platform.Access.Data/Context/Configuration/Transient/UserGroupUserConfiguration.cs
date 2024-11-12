using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Domain.Entities.Transient;

namespace Reapit.Platform.Access.Data.Context.Configuration.Transient;

public class UserGroupUserConfiguration : IEntityTypeConfiguration<UserGroupUser>
{
    public void Configure(EntityTypeBuilder<UserGroupUser> builder)
    {
        builder.ToTable("user_group_users");

        builder.HasKey(entity => new { entity.UserGroupId, entity.OrganisationUserId });
        
        builder.HasQueryFilter(entity => entity.UserGroup.DateDeleted == null);

        builder.Property(entity => entity.UserGroupId)
            .HasColumnName("user_group_id")
            .HasMaxLength(36);

        builder.Property(entity => entity.OrganisationUserId)
            .HasColumnName("organisation_user_id");

        builder.HasOne(entity => entity.UserGroup)
            .WithMany(userGroup => userGroup.UserGroupUsers)
            .HasForeignKey(entity => entity.UserGroupId);
        
        builder.HasOne(entity => entity.OrganisationUser)
            .WithMany(organisationUser => organisationUser.UserGroupUsers)
            .HasForeignKey(entity => entity.OrganisationUserId);
    }
}