using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HFiles.Data.Migrations
{
    public partial class NewColumnsUserFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "UserFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserFiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserFiles");
        }
    }
}
