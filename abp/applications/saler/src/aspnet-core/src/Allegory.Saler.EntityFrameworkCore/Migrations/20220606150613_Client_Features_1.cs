using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class Client_Features_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AppClients",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "AppClients",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxOffice",
                table: "AppClients",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "AppClients",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_AppClients_IdentityNumber",
                table: "AppClients",
                column: "IdentityNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppClients_IdentityNumber",
                table: "AppClients");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AppClients");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "AppClients");

            migrationBuilder.DropColumn(
                name: "TaxOffice",
                table: "AppClients");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppClients");
        }
    }
}
