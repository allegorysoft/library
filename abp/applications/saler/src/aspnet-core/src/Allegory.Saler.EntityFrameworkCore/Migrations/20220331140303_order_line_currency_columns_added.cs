using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class order_line_currency_columns_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "AppOrderLines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrencyRate",
                table: "AppOrderLines",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrderLines_CurrencyId",
                table: "AppOrderLines",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrderLines_AppCurrencies_CurrencyId",
                table: "AppOrderLines",
                column: "CurrencyId",
                principalTable: "AppCurrencies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrderLines_AppCurrencies_CurrencyId",
                table: "AppOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_AppOrderLines_CurrencyId",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "CurrencyRate",
                table: "AppOrderLines");
        }
    }
}
