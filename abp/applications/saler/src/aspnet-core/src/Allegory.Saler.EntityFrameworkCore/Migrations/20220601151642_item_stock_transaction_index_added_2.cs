using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class item_stock_transaction_index_added_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppItemStockTransactions_ItemId_Date",
                table: "AppItemStockTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_ItemId",
                table: "AppItemStockTransactions",
                column: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppItemStockTransactions_ItemId",
                table: "AppItemStockTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_ItemId_Date",
                table: "AppItemStockTransactions",
                columns: new[] { "ItemId", "Date" })
                .Annotation("SqlServer:Include", new[] { "Quantity", "IsOutput", "Statu" });
        }
    }
}
