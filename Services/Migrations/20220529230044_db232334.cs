using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Services.Migrations
{
    public partial class db232334 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Test",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sayi = table.Column<int>(type: "int", nullable: false),
                    CreaDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreaUser = table.Column<int>(type: "int", nullable: false),
                    ModUser = table.Column<int>(type: "int", nullable: true),
                    ModDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsStatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Test");
        }
    }
}
