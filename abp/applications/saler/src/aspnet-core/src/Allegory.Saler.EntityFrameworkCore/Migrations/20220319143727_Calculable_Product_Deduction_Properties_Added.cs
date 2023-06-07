using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class Calculable_Product_Deduction_Properties_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeductionCode",
                table: "AppOrderLines",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DeductionPart1",
                table: "AppOrderLines",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DeductionPart2",
                table: "AppOrderLines",
                type: "smallint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeductionCode",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "DeductionPart1",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "DeductionPart2",
                table: "AppOrderLines");
        }
    }
}
