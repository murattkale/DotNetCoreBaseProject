using Microsoft.EntityFrameworkCore.Migrations;

namespace Services.Migrations
{
    public partial class db12323efdfdfdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddColumn<string>(
                name: "MainList",
                table: "Forms",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainList",
                table: "Forms");

          
        }
    }
}
