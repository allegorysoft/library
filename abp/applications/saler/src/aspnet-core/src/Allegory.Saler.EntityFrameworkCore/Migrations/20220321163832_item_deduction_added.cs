using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class item_deduction_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeductionCode",
                table: "AppItems",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DeductionPart1",
                table: "AppItems",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DeductionPart2",
                table: "AppItems",
                type: "smallint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeductionCode",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "DeductionPart1",
                table: "AppItems");

            migrationBuilder.DropColumn(
                name: "DeductionPart2",
                table: "AppItems");
        }
    }
}
