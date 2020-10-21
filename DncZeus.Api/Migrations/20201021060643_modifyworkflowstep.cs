using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class modifyworkflowstep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsManual",
                table: "WorkflowStep",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsManual",
                table: "WorkflowStep");
        }
    }
}
