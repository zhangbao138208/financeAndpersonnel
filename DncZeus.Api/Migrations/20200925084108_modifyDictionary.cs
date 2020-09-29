using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class modifyDictionary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "SystemDictionary");

            migrationBuilder.AlterColumn<string>(
                name: "TypeCode",
                table: "SystemDictionary",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemDictionary_Code",
                table: "SystemDictionary",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemDictionary_TypeCode",
                table: "SystemDictionary",
                column: "TypeCode");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemDictionary_SystemDicType_TypeCode",
                table: "SystemDictionary",
                column: "TypeCode",
                principalTable: "SystemDicType",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemDictionary_SystemDicType_TypeCode",
                table: "SystemDictionary");

            migrationBuilder.DropIndex(
                name: "IX_SystemDictionary_Code",
                table: "SystemDictionary");

            migrationBuilder.DropIndex(
                name: "IX_SystemDictionary_TypeCode",
                table: "SystemDictionary");

            migrationBuilder.AlterColumn<string>(
                name: "TypeCode",
                table: "SystemDictionary",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "SystemDictionary",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
