using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class currency_daily_exchanges_table_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCurrencyDailyExchanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate1 = table.Column<double>(type: "float", nullable: false),
                    Rate2 = table.Column<double>(type: "float", nullable: false),
                    Rate3 = table.Column<double>(type: "float", nullable: false),
                    Rate4 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCurrencyDailyExchanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCurrencyDailyExchanges_AppCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "AppCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCurrencyDailyExchanges_CurrencyId",
                table: "AppCurrencyDailyExchanges",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCurrencyDailyExchanges_Date_CurrencyId",
                table: "AppCurrencyDailyExchanges",
                columns: new[] { "Date", "CurrencyId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCurrencyDailyExchanges");
        }
    }
}
