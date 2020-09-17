using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class rmDepartmentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserDepartment",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserDepartment");
        }
    }
}
