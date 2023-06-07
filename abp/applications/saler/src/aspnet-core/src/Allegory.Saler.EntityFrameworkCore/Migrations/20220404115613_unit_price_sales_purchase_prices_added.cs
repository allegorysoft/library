using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class unit_price_sales_purchase_prices_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "AppUnitPrices",
                newName: "SalesPrice");

            migrationBuilder.AddColumn<double>(
                name: "PurchasePrice",
                table: "AppUnitPrices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "AppUnitPrices");

            migrationBuilder.RenameColumn(
                name: "SalesPrice",
                table: "AppUnitPrices",
                newName: "Price");
        }
    }
}
