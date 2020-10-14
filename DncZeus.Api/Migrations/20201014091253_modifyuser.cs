using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class modifyuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DncUser_UserDepartment_PositionCode",
                table: "DncUser");

            migrationBuilder.AddForeignKey(
                name: "FK_DncUser_UserPosition_PositionCode",
                table: "DncUser",
                column: "PositionCode",
                principalTable: "UserPosition",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DncUser_UserPosition_PositionCode",
                table: "DncUser");

            migrationBuilder.AddForeignKey(
                name: "FK_DncUser_UserDepartment_PositionCode",
                table: "DncUser",
                column: "PositionCode",
                principalTable: "UserDepartment",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
