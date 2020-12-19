using Microsoft.EntityFrameworkCore.Migrations;

namespace ServerDatabaseSystem.Migrations
{
    public partial class updatedatabasedesign07122020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "Notifications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "Notifications");
        }
    }
}
