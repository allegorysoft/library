using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class service_deduction_columns_created : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeductionCode",
                table: "AppServices",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "PurchaseDeductionPart1",
                table: "AppServices",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "PurchaseDeductionPart2",
                table: "AppServices",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SalesDeductionPart1",
                table: "AppServices",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SalesDeductionPart2",
                table: "AppServices",
                type: "smallint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeductionCode",
                table: "AppServices");

            migrationBuilder.DropColumn(
                name: "PurchaseDeductionPart1",
                table: "AppServices");

            migrationBuilder.DropColumn(
                name: "PurchaseDeductionPart2",
                table: "AppServices");

            migrationBuilder.DropColumn(
                name: "SalesDeductionPart1",
                table: "AppServices");

            migrationBuilder.DropColumn(
                name: "SalesDeductionPart2",
                table: "AppServices");
        }
    }
}
