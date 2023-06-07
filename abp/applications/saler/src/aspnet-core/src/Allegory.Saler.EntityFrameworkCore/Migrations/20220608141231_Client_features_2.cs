using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class Client_features_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EMail",
                table: "AppClients",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KepAddress",
                table: "AppClients",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone1",
                table: "AppClients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone2",
                table: "AppClients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone3",
                table: "AppClients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EMail",
                table: "AppClients");

            migrationBuilder.DropColumn(
                name: "KepAddress",
                table: "AppClients");

            migrationBuilder.DropColumn(
                name: "Phone1",
                table: "AppClients");

            migrationBuilder.DropColumn(
                name: "Phone2",
                table: "AppClients");

            migrationBuilder.DropColumn(
                name: "Phone3",
                table: "AppClients");
        }
    }
}
