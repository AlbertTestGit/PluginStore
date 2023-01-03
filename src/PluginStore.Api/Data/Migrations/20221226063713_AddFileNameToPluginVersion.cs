using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginStore.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNameToPluginVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "PluginVersions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "PluginVersions");
        }
    }
}
