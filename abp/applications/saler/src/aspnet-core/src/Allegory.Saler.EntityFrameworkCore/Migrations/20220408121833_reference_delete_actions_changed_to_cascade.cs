using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class reference_delete_actions_changed_to_cascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppClientUsers_AppClients_ClientId",
                table: "AppClientUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppClients_ClientId",
                table: "AppUnitPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppCurrencies_CurrencyId",
                table: "AppUnitPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppUnits_UnitId",
                table: "AppUnitPrices");

            migrationBuilder.AddForeignKey(
                name: "FK_AppClientUsers_AppClients_ClientId",
                table: "AppClientUsers",
                column: "ClientId",
                principalTable: "AppClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_AppUnitPrices_AppUnits_UnitId",
                table: "AppUnitPrices",
                column: "UnitId",
                principalTable: "AppUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppClientUsers_AppClients_ClientId",
                table: "AppClientUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppClients_ClientId",
                table: "AppUnitPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppCurrencies_CurrencyId",
                table: "AppUnitPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppUnits_UnitId",
                table: "AppUnitPrices");

            migrationBuilder.AddForeignKey(
                name: "FK_AppClientUsers_AppClients_ClientId",
                table: "AppClientUsers",
                column: "ClientId",
                principalTable: "AppClients",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_AppUnitPrices_AppUnits_UnitId",
                table: "AppUnitPrices",
                column: "UnitId",
                principalTable: "AppUnits",
                principalColumn: "Id");
        }
    }
}
