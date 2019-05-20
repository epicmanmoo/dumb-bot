using Microsoft.EntityFrameworkCore.Migrations;

namespace botTesting.Migrations
{
    public partial class M3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "joinmsgs",
                table: "Spclcmds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "leavemsgs",
                table: "Spclcmds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "prefix",
                table: "Spclcmds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "joinmsgs",
                table: "Spclcmds");

            migrationBuilder.DropColumn(
                name: "leavemsgs",
                table: "Spclcmds");

            migrationBuilder.DropColumn(
                name: "prefix",
                table: "Spclcmds");
        }
    }
}
