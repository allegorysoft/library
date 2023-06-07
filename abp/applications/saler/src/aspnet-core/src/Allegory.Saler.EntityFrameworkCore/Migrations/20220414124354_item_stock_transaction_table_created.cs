using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class item_stock_transaction_table_created : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppItemStockTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    TransactionParentId = table.Column<int>(type: "int", nullable: false),
                    IsOutput = table.Column<bool>(type: "bit", nullable: false),
                    IsReserved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppItemStockTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppItemStockTransactions_AppItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "AppItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_Date",
                table: "AppItemStockTransactions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_ItemId",
                table: "AppItemStockTransactions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_Type_TransactionId_IsOutput",
                table: "AppItemStockTransactions",
                columns: new[] { "Type", "TransactionId", "IsOutput" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppItemStockTransactions_Type_TransactionParentId",
                table: "AppItemStockTransactions",
                columns: new[] { "Type", "TransactionParentId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppItemStockTransactions");
        }
    }
}
