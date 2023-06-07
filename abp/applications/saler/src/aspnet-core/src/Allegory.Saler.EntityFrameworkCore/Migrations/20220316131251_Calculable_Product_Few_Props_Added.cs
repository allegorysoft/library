using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class Calculable_Product_Few_Props_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CalculatedTotal",
                table: "AppOrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountTotal",
                table: "AppOrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculatedTotal",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "DiscountTotal",
                table: "AppOrderLines");
        }
    }
}
