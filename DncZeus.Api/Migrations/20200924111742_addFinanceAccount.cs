using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class addFinanceAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinanceAccount",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    Holder = table.Column<string>(nullable: true),
                    DepartmentCode = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Account = table.Column<string>(nullable: true),
                    Owner = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceAccount", x => x.Code);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinanceAccount");
        }
    }
}
