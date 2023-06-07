using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class item_deduction_map_changed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeductionPart2",
                table: "AppItems",
                newName: "SalesDeductionPart2");

            migrationBuilder.RenameColumn(
                name: "DeductionPart1",
                table: "AppItems",
                newName: "SalesDeductionPart1");

            migrationBuilder.AddColumn<short>(
                name: "PurchaseDeductionPart1",
                table: "AppItems",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "PurchaseDeductionPart2",
                table: "AppItems",
                type: "smallint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseDeductionPart1",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "PurchaseDeductionPart2",
                table: "AppItems");

            migrationBuilder.RenameColumn(
                name: "SalesDeductionPart2",
                table: "AppItems",
                newName: "DeductionPart2");

            migrationBuilder.RenameColumn(
                name: "SalesDeductionPart1",
                table: "AppItems",
                newName: "DeductionPart1");
        }
    }
}
