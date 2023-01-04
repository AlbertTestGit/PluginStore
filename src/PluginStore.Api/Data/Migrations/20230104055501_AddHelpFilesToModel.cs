using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginStore.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHelpFilesToModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HelpFileEn",
                table: "PluginVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HelpFileKz",
                table: "PluginVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HelpFileRu",
                table: "PluginVersions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HelpFileEn",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "HelpFileKz",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "HelpFileRu",
                table: "PluginVersions");
        }
    }
}
