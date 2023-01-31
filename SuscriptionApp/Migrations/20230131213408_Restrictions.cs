﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuscriptionApp.Migrations
{
    public partial class Restrictions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DomainRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainRestrictions_KeysAPI_KeyId",
                        column: x => x.KeyId,
                        principalTable: "KeysAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IPRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPRestrictions_KeysAPI_KeyId",
                        column: x => x.KeyId,
                        principalTable: "KeysAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainRestrictions_KeyId",
                table: "DomainRestrictions",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_IPRestrictions_KeyId",
                table: "IPRestrictions",
                column: "KeyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainRestrictions");

            migrationBuilder.DropTable(
                name: "IPRestrictions");
        }
    }
}
