using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class unit_price_relation_changed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppClients_ClientId",
                table: "AppUnitPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppCurrencies_CurrencyId",
                table: "AppUnitPrices");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUnitPrices_AppClients_ClientId",
                table: "AppUnitPrices",
                column: "ClientId",
                principalTable: "AppClients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUnitPrices_AppCurrencies_CurrencyId",
                table: "AppUnitPrices",
                column: "CurrencyId",
                principalTable: "AppCurrencies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppClients_ClientId",
                table: "AppUnitPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppCurrencies_CurrencyId",
                table: "AppUnitPrices");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUnitPrices_AppClients_ClientId",
                table: "AppUnitPrices",
                column: "ClientId",
                principalTable: "AppClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUnitPrices_AppCurrencies_CurrencyId",
                table: "AppUnitPrices",
                column: "CurrencyId",
                principalTable: "AppCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
