using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Migrations
{
    /// <inheritdoc />
    public partial class intit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContextId",
                table: "LogbookEntries");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "LogbookEntries");

            migrationBuilder.DropColumn(
                name: "EntityPicture",
                table: "LogbookEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContextId",
                table: "LogbookEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "LogbookEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EntityPicture",
                table: "LogbookEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
