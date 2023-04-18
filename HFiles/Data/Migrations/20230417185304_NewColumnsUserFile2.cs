using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HFiles.Data.Migrations
{
    public partial class NewColumnsUserFile2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "UserFiles",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "UserFiles");
        }
    }
}
