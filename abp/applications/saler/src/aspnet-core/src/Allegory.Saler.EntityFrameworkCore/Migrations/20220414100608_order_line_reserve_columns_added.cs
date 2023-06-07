using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    public partial class order_line_reserve_columns_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReserveDate",
                table: "AppOrderLines",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ReserveQuantity",
                table: "AppOrderLines",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReserveDate",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "ReserveQuantity",
                table: "AppOrderLines");
        }
    }
}
