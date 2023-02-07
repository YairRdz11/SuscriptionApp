using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuscriptionApp.Migrations
{
    public partial class baduser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BadUser",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BadUser",
                table: "AspNetUsers");
        }
    }
}
