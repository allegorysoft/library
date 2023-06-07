using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class order_currency_columns_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "AppOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrencyRate",
                table: "AppOrders",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_CurrencyId",
                table: "AppOrders",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrders_AppCurrencies_CurrencyId",
                table: "AppOrders",
                column: "CurrencyId",
                principalTable: "AppCurrencies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrders_AppCurrencies_CurrencyId",
                table: "AppOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppOrders_CurrencyId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "CurrencyRate",
                table: "AppOrders");
        }
    }
}
