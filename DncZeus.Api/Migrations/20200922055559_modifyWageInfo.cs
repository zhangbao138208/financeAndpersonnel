using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class modifyWageInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Other",
                table: "WageInfo");

            migrationBuilder.DropColumn(
                name: "OtherRemark",
                table: "WageInfo");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalWage",
                table: "WageInfo",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Additions",
                table: "WageInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Additions",
                table: "WageInfo");

            migrationBuilder.AlterColumn<string>(
                name: "TotalWage",
                table: "WageInfo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Other",
                table: "WageInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherRemark",
                table: "WageInfo",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
