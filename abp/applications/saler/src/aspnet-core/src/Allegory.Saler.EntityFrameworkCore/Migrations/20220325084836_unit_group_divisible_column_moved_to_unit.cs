using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class unit_group_divisible_column_moved_to_unit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Divisible",
                table: "AppUnitGroups");

            migrationBuilder.AddColumn<bool>(
                name: "Divisible",
                table: "AppUnits",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Divisible",
                table: "AppUnits");

            migrationBuilder.AddColumn<bool>(
                name: "Divisible",
                table: "AppUnitGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
