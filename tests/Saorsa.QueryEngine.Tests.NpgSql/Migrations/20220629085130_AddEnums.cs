using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saorsa.QueryEngine.Tests.NpgSql.Migrations
{
    public partial class AddEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LatestLogonType",
                table: "Users",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestLogonType",
                table: "Users");
        }
    }
}
