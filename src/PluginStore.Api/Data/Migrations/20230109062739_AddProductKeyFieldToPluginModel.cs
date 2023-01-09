using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginStore.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductKeyFieldToPluginModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductKey",
                table: "Plugins",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductKey",
                table: "Plugins");
        }
    }
}
