using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class item_stock_transaction_index_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppItemStockTransactions_Date",
                table: "AppItemStockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AppItemStockTransactions_ItemId_Statu",
                table: "AppItemStockTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_ItemId_Date",
                table: "AppItemStockTransactions",
                columns: new[] { "ItemId", "Date" })
                .Annotation("SqlServer:Include", new[] { "Quantity", "IsOutput", "Statu" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppItemStockTransactions_ItemId_Date",
                table: "AppItemStockTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_Date",
                table: "AppItemStockTransactions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_ItemId_Statu",
                table: "AppItemStockTransactions",
                columns: new[] { "ItemId", "Statu" });
        }
    }
}
