﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Reapit.Platform.Access.Data.Context;

#nullable disable

namespace Reapit.Platform.Access.Data.Context.Migrations
{
    [DbContext(typeof(AccessDbContext))]
    [Migration("20241115082014_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Reapit.Platform.Access.Domain.Entities.Group", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)")
                        .HasColumnName("id");

                    b.Property<long>("Cursor")
                        .HasColumnType("bigint")
                        .HasColumnName("cursor");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("date_created");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("deleted");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("date_modified");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.Property<string>("OrganisationId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("organisation_id");

                    b.HasKey("Id");

                    b.HasIndex("Cursor")
                        .IsUnique();

                    b.HasIndex("DateCreated");

                    b.HasIndex("DateDeleted");

                    b.HasIndex("DateModified");

                    b.HasIndex("OrganisationId", "Name", "DateDeleted")
                        .IsUnique();

                    b.ToTable("groups", (string)null);
                });

            modelBuilder.Entity("Reapit.Platform.Access.Domain.Entities.Instance", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)")
                        .HasColumnName("id");

                    b.Property<long>("Cursor")
                        .HasColumnType("bigint")
                        .HasColumnName("cursor");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("date_created");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("deleted");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("date_modified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.Property<string>("OrganisationId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)")
                        .HasColumnName("organisation_id");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)")
                        .HasColumnName("product_id");

                    b.HasKey("Id");

                    b.HasIndex("Cursor")
                        .IsUnique();

                    b.HasIndex("DateCreated");

                    b.HasIndex("DateDeleted");

                    b.HasIndex("DateModified");

                    b.HasIndex("OrganisationId");

                    b.HasIndex("ProductId");

                    b.ToTable("instances", (string)null);
                });

            modelBuilder.Entity("Reapit.Platform.Access.Domain.Entities.Organisation", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("DateLastSynchronised")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("last_sync");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("organisations", (string)null);
                });

            modelBuilder.Entity("Reapit.Platform.Access.Domain.Entities.Product", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)")
                        .HasColumnName("id");

                    b.Property<long>("Cursor")
                        .HasColumnType("bigint")
                        .HasColumnName("cursor");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("date_created");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("deleted");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("date_modified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("Cursor")
                        .IsUnique();

                    b.HasIndex("DateCreated");

                    b.HasIndex("DateDeleted");

                    b.HasIndex("DateModified");

                    b.ToTable("products", (string)null);
                });

            modelBuilder.Entity("Reapit.Platform.Access.Domain.Entities.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)")
                        .HasColumnName("id");

                    b.Property<long>("Cursor")
                        .HasColumnType("bigint")
                        .HasColumnName("cursor");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("date_created");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("deleted");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("date_modified");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("Cursor")
                        .IsUnique();

                    b.HasIndex("DateCreated");

                    b.HasIndex("DateDeleted");

                    b.HasIndex("DateModified");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("roles", (string)null);
                });

            modelBuilder.Entity("Reapit.Platform.Access.Domain.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("DateLastSynchronised")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("last_sync");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("groupUsers", b =>
                {
                    b.Property<string>("groupId")
                        .HasColumnType("varchar(36)");

                    b.Property<string>("userId")
                        .HasColumnType("varchar(100)");

                    b.HasKey("groupId", "userId");

                    b.HasIndex("userId");

                    b.ToTable("groupUsers");
                });

            modelBuilder.Entity("organisationUsers", b =>
                {
                    b.Property<string>("organisationId")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("userId")
                        .HasColumnType("varchar(100)");

                    b.HasKey("organisationId", "userId");

                    b.HasIndex("userId");

                    b.ToTable("organisationUsers");
                });

            modelBuilder.Entity("userRoles", b =>
                {
                    b.Property<string>("roleId")
                        .HasColumnType("varchar(36)");

                    b.Property<string>("userId")
                        .HasColumnType("varchar(100)");

                    b.HasKey("roleId", "userId");

                    b.HasIndex("userId");

                    b.ToTable("userRoles");
                });

            modelBuilder.Entity("Reapit.Platform.Access.Domain.Entities.Group", b =>
                {
                    b.HasOne("Reapit.Platform.Access.Domain.Entities.Organisation", "Organisation")
                        .WithMany("Groups")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Reapit.Platform.Access.Domain.Entities.Instance", b =>
                {
                    b.HasOne("Reapit.Platform.Access.Domain.Entities.Organisation", "Organisation")
                        .WithMany("Instances")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Reapit.Platform.Access.Domain.Entities.Product", "Product")
                        .WithMany("Instances")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("groupUsers", b =>
                {
                    b.HasOne("Reapit.Platform.Access.Domain.Entities.Group", null)
                        .WithMany()
                        .HasForeignKey("groupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Reapit.Platform.Access.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("organisationUsers", b =>
                {
                    b.HasOne("Reapit.Platform.Access.Domain.Entities.Organisation", null)
                        .WithMany()
                        .HasForeignKey("organisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Reapit.Platform.Access.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("userRoles", b =>
                {
                    b.HasOne("Reapit.Platform.Access.Domain.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("roleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Reapit.Platform.Access.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Reapit.Platform.Access.Domain.Entities.Organisation", b =>
                {
                    b.Navigation("Groups");

                    b.Navigation("Instances");
                });

            modelBuilder.Entity("Reapit.Platform.Access.Domain.Entities.Product", b =>
                {
                    b.Navigation("Instances");
                });
#pragma warning restore 612, 618
        }
    }
}