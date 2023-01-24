using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuscriptionApp.Migrations
{
    public partial class userIdmodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeysAPI_AspNetUsers_UserId1",
                table: "KeysAPI");

            migrationBuilder.DropIndex(
                name: "IX_KeysAPI_UserId1",
                table: "KeysAPI");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "KeysAPI");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "KeysAPI",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_KeysAPI_UserId",
                table: "KeysAPI",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_KeysAPI_AspNetUsers_UserId",
                table: "KeysAPI",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeysAPI_AspNetUsers_UserId",
                table: "KeysAPI");

            migrationBuilder.DropIndex(
                name: "IX_KeysAPI_UserId",
                table: "KeysAPI");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "KeysAPI",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "KeysAPI",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KeysAPI_UserId1",
                table: "KeysAPI",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_KeysAPI_AspNetUsers_UserId1",
                table: "KeysAPI",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
