using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.Migrations
{
    /// <inheritdoc />
    public partial class init42 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "role",
                table: "OtpEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "OtpEntries",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
