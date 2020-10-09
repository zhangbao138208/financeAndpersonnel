using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class addWorkflowReceicer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkflowCode",
                table: "WorkflowList");

            migrationBuilder.CreateTable(
                name: "WorkflowReceiver",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowCode = table.Column<string>(nullable: true),
                    TemplateCode = table.Column<string>(nullable: true),
                    StepCode = table.Column<string>(nullable: true),
                    User = table.Column<Guid>(nullable: false),
                    IsCheck = table.Column<bool>(nullable: false),
                    CheckDate = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    CreateUser = table.Column<Guid>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowReceiver", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowReceiver");

            migrationBuilder.AddColumn<string>(
                name: "WorkflowCode",
                table: "WorkflowList",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
