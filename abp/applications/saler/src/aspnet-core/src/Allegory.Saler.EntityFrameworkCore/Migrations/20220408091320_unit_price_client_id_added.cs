using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class unit_price_client_id_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "AppUnitPrices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUnitPrices_ClientId",
                table: "AppUnitPrices",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUnitPrices_AppClients_ClientId",
                table: "AppUnitPrices",
                column: "ClientId",
                principalTable: "AppClients",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUnitPrices_AppClients_ClientId",
                table: "AppUnitPrices");

            migrationBuilder.DropIndex(
                name: "IX_AppUnitPrices_ClientId",
                table: "AppUnitPrices");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AppUnitPrices");
        }
    }
}
