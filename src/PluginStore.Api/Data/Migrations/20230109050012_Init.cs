using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PluginStore.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    PluginId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DeveloperKey = table.Column<string>(type: "text", nullable: true),
                    PetrelVersion = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.PluginId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    LicenseNumber = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "PluginVersions",
                columns: table => new
                {
                    PluginVersionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    HelpFileEn = table.Column<string>(type: "text", nullable: true),
                    HelpFileRu = table.Column<string>(type: "text", nullable: true),
                    HelpFileKz = table.Column<string>(type: "text", nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GitLink = table.Column<string>(type: "text", nullable: false),
                    Beta = table.Column<bool>(type: "boolean", nullable: false),
                    Deprecated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PluginId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginVersions", x => x.PluginVersionId);
                    table.ForeignKey(
                        name: "FK_PluginVersions_Plugins_PluginId",
                        column: x => x.PluginId,
                        principalTable: "Plugins",
                        principalColumn: "PluginId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PluginVersions_Users_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PluginVersions_AuthorUserId",
                table: "PluginVersions",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PluginVersions_PluginId",
                table: "PluginVersions",
                column: "PluginId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PluginVersions");

            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
