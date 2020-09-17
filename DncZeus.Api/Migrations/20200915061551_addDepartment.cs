using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class addDepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDepartment",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserGuid = table.Column<Guid>(nullable: false),
                    CreatedByUserName = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedByUserGuid = table.Column<Guid>(nullable: true),
                    ModifiedByUserName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Phone1 = table.Column<string>(nullable: true),
                    Phone2 = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    ParentCode = table.Column<string>(nullable: true),
                    LevelID = table.Column<int>(nullable: false),
                    SortID = table.Column<int>(nullable: false),
                    TypeID = table.Column<int>(nullable: false),
                    Province = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    County = table.Column<string>(nullable: true),
                    Zone = table.Column<string>(nullable: true),
                    Manager = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    Monday = table.Column<string>(nullable: true),
                    Tuesday = table.Column<string>(nullable: true),
                    Wednesday = table.Column<string>(nullable: true),
                    Thursday = table.Column<string>(nullable: true),
                    Friday = table.Column<string>(nullable: true),
                    Saturday = table.Column<string>(nullable: true),
                    Sunday = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDepartment", x => x.Code);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDepartment");
        }
    }
}
