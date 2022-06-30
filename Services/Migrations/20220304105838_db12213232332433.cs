using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Services.Migrations
{
    public partial class db12213232332433 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAdressNote",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MainProductId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CouponType = table.Column<int>(type: "int", nullable: false),
                    CouponValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinBasket = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Limit = table.Column<int>(type: "int", nullable: true),
                    Used = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_Coupon", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_CouponId",
                table: "Order",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_MainProductId",
                table: "Order",
                column: "MainProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Coupon_CouponId",
                table: "Order",
                column: "CouponId",
                principalTable: "Coupon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Product_MainProductId",
                table: "Order",
                column: "MainProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Coupon_CouponId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Product_MainProductId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropIndex(
                name: "IX_Order_CouponId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_MainProductId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "MainProductId",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "BillingAdressNote",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
