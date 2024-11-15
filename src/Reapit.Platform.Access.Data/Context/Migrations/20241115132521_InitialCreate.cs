using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reapit.Platform.Access.Data.Context.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "organisations",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_sync = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organisations", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    deleted = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    deleted = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    last_sync = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    organisation_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    deleted = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_groups_organisations_organisation_id",
                        column: x => x.organisation_id,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "instances",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    product_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    organisation_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cursor = table.Column<long>(type: "bigint", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    deleted = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instances", x => x.id);
                    table.ForeignKey(
                        name: "FK_instances_organisations_organisation_id",
                        column: x => x.organisation_id,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_instances_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "organisationUsers",
                columns: table => new
                {
                    organisationId = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userId = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organisationUsers", x => new { x.organisationId, x.userId });
                    table.ForeignKey(
                        name: "FK_organisationUsers_organisations_organisationId",
                        column: x => x.organisationId,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_organisationUsers_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "userRoles",
                columns: table => new
                {
                    roleId = table.Column<string>(type: "varchar(36)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userId = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userRoles", x => new { x.roleId, x.userId });
                    table.ForeignKey(
                        name: "FK_userRoles_roles_roleId",
                        column: x => x.roleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userRoles_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "groupUsers",
                columns: table => new
                {
                    groupId = table.Column<string>(type: "varchar(36)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userId = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groupUsers", x => new { x.groupId, x.userId });
                    table.ForeignKey(
                        name: "FK_groupUsers_groups_groupId",
                        column: x => x.groupId,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_groupUsers_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_groups_cursor",
                table: "groups",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_groups_date_created",
                table: "groups",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_groups_date_modified",
                table: "groups",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_groups_deleted",
                table: "groups",
                column: "deleted");

            migrationBuilder.CreateIndex(
                name: "IX_groups_organisation_id_name_deleted",
                table: "groups",
                columns: new[] { "organisation_id", "name", "deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_groupUsers_userId",
                table: "groupUsers",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_instances_cursor",
                table: "instances",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_instances_date_created",
                table: "instances",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_instances_date_modified",
                table: "instances",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_instances_deleted",
                table: "instances",
                column: "deleted");

            migrationBuilder.CreateIndex(
                name: "IX_instances_organisation_id",
                table: "instances",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_instances_product_id",
                table: "instances",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_organisationUsers_userId",
                table: "organisationUsers",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_products_cursor",
                table: "products",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_date_created",
                table: "products",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_products_date_modified",
                table: "products",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_products_deleted",
                table: "products",
                column: "deleted");

            migrationBuilder.CreateIndex(
                name: "IX_roles_cursor",
                table: "roles",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_date_created",
                table: "roles",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_roles_date_modified",
                table: "roles",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_roles_deleted",
                table: "roles",
                column: "deleted");

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userRoles_userId",
                table: "userRoles",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "groupUsers");

            migrationBuilder.DropTable(
                name: "instances");

            migrationBuilder.DropTable(
                name: "organisationUsers");

            migrationBuilder.DropTable(
                name: "userRoles");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "organisations");
        }
    }
}
