using Microsoft.EntityFrameworkCore.Migrations;

namespace DncZeus.Api.Migrations
{
    public partial class modifyrole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentCode",
                table: "DncRole",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentCode",
                table: "DncRole");
        }
    }
}
