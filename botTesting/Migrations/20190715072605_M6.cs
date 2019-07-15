using Microsoft.EntityFrameworkCore.Migrations;

namespace botTesting.Migrations
{
    public partial class M6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "age",
                table: "welcomes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "desc",
                table: "welcomes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "favcolor",
                table: "welcomes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "favfood",
                table: "welcomes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "welcomes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "welcomes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "plurals",
                table: "welcomes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "age",
                table: "welcomes");

            migrationBuilder.DropColumn(
                name: "desc",
                table: "welcomes");

            migrationBuilder.DropColumn(
                name: "favcolor",
                table: "welcomes");

            migrationBuilder.DropColumn(
                name: "favfood",
                table: "welcomes");

            migrationBuilder.DropColumn(
                name: "location",
                table: "welcomes");

            migrationBuilder.DropColumn(
                name: "name",
                table: "welcomes");

            migrationBuilder.DropColumn(
                name: "plurals",
                table: "welcomes");
        }
    }
}
