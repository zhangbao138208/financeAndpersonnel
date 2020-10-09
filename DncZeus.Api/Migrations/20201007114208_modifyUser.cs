using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class modifyUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "DncUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PositionCode",
                table: "DncUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelegramBotToken",
                table: "DncUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelegramChatId",
                table: "DncUser",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DncUser_DepartmentCode",
                table: "DncUser",
                column: "DepartmentCode");

            migrationBuilder.CreateIndex(
                name: "IX_DncUser_PositionCode",
                table: "DncUser",
                column: "PositionCode");

            migrationBuilder.AddForeignKey(
                name: "FK_DncUser_UserDepartment_DepartmentCode",
                table: "DncUser",
                column: "DepartmentCode",
                principalTable: "UserDepartment",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DncUser_UserDepartment_PositionCode",
                table: "DncUser",
                column: "PositionCode",
                principalTable: "UserDepartment",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DncUser_UserDepartment_DepartmentCode",
                table: "DncUser");

            migrationBuilder.DropForeignKey(
                name: "FK_DncUser_UserDepartment_PositionCode",
                table: "DncUser");

            migrationBuilder.DropIndex(
                name: "IX_DncUser_DepartmentCode",
                table: "DncUser");

            migrationBuilder.DropIndex(
                name: "IX_DncUser_PositionCode",
                table: "DncUser");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "DncUser");

            migrationBuilder.DropColumn(
                name: "PositionCode",
                table: "DncUser");

            migrationBuilder.DropColumn(
                name: "TelegramBotToken",
                table: "DncUser");

            migrationBuilder.DropColumn(
                name: "TelegramChatId",
                table: "DncUser");
        }
    }
}
