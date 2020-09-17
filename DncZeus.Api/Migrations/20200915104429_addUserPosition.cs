using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class addUserPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPosition",
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
                    Name = table.Column<string>(nullable: true),
                    ParentCode = table.Column<string>(nullable: true),
                    LevelID = table.Column<int>(nullable: false),
                    SortID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPosition", x => x.Code);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPosition");
        }
    }
}
