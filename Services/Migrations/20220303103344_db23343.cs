using Microsoft.EntityFrameworkCore.Migrations;

namespace Services.Migrations
{
    public partial class db23343 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingAdressNote",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAdressNote",
                table: "Order");
        }
    }
}
