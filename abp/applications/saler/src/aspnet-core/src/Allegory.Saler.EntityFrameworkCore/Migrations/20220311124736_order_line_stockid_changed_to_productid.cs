using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class order_line_stockid_changed_to_productid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StockId",
                table: "AppOrderLines",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_AppOrderLines_Type_StockId",
                table: "AppOrderLines",
                newName: "IX_AppOrderLines_Type_ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "AppOrderLines",
                newName: "StockId");

            migrationBuilder.RenameIndex(
                name: "IX_AppOrderLines_Type_ProductId",
                table: "AppOrderLines",
                newName: "IX_AppOrderLines_Type_StockId");
        }
    }
}
