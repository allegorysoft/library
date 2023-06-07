using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class orders_total_columns_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalDiscount",
                table: "AppOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalGross",
                table: "AppOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalVatAmount",
                table: "AppOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalVatBase",
                table: "AppOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalDiscount",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "TotalGross",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "TotalVatAmount",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "TotalVatBase",
                table: "AppOrders");
        }
    }
}
