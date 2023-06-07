using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class currencies_table_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCurrencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    Symbol = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCurrencies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_Date",
                table: "AppOrders",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_AppCurrencies_Code",
                table: "AppCurrencies",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCurrencies");

            migrationBuilder.DropIndex(
                name: "IX_AppOrders_Date",
                table: "AppOrders");
        }
    }
}
