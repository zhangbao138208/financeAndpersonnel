using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class addWageInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WageInfo",
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
                    UserGuid = table.Column<Guid>(nullable: false),
                    RealName = table.Column<string>(nullable: true),
                    DepartmentCode = table.Column<string>(nullable: true),
                    PositionCode = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    BaseWage = table.Column<decimal>(nullable: true),
                    WorkDays = table.Column<int>(nullable: true),
                    OTWage = table.Column<decimal>(nullable: true),
                    OTDays = table.Column<int>(nullable: true),
                    PerformanceWage = table.Column<decimal>(nullable: true),
                    ReissueWage = table.Column<decimal>(nullable: true),
                    Commission = table.Column<decimal>(nullable: true),
                    Bonus = table.Column<decimal>(nullable: true),
                    Subsidy = table.Column<decimal>(nullable: true),
                    SocialSecurity = table.Column<decimal>(nullable: true),
                    AccumulationFund = table.Column<decimal>(nullable: true),
                    IncomeTax = table.Column<decimal>(nullable: true),
                    Deductions = table.Column<string>(nullable: true),
                    Other = table.Column<string>(nullable: true),
                    OtherRemark = table.Column<string>(nullable: true),
                    TotalWage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WageInfo", x => x.Code);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WageInfo");
        }
    }
}
