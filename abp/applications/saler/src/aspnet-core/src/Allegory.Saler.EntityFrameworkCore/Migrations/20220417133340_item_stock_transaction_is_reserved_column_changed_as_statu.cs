using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class item_stock_transaction_is_reserved_column_changed_as_statu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppItemStockTransactions_ItemId",
                table: "AppItemStockTransactions");

            migrationBuilder.DropColumn(
                name: "IsReserved",
                table: "AppItemStockTransactions");

            migrationBuilder.AddColumn<byte>(
                name: "Statu",
                table: "AppItemStockTransactions",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_ItemId_Statu",
                table: "AppItemStockTransactions",
                columns: new[] { "ItemId", "Statu" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppItemStockTransactions_ItemId_Statu",
                table: "AppItemStockTransactions");

            migrationBuilder.DropColumn(
                name: "Statu",
                table: "AppItemStockTransactions");

            migrationBuilder.AddColumn<bool>(
                name: "IsReserved",
                table: "AppItemStockTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_ItemId",
                table: "AppItemStockTransactions",
                column: "ItemId");
        }
    }
}
