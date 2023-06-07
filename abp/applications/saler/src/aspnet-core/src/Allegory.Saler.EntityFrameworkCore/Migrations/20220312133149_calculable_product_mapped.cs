using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class calculable_product_mapped : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVatIncluded",
                table: "AppOrderLines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "AppOrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Total",
                table: "AppOrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VatAmount",
                table: "AppOrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VatBase",
                table: "AppOrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VatRate",
                table: "AppOrderLines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVatIncluded",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "VatBase",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "AppOrderLines");
        }
    }
}
