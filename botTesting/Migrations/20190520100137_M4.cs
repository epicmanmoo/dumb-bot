using Microsoft.EntityFrameworkCore.Migrations;

namespace botTesting.Migrations
{
    public partial class M4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "leavemsgs",
                table: "Spclcmds",
                newName: "Leavemsgs");

            migrationBuilder.RenameColumn(
                name: "joinmsgs",
                table: "Spclcmds",
                newName: "Joinmsgs");

            migrationBuilder.RenameColumn(
                name: "guildId",
                table: "Spclcmds",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "prefix",
                table: "Spclcmds",
                newName: "NameOfBot");

            migrationBuilder.AddColumn<string>(
                name: "MsgPrefix",
                table: "Spclcmds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MsgPrefix",
                table: "Spclcmds");

            migrationBuilder.RenameColumn(
                name: "Leavemsgs",
                table: "Spclcmds",
                newName: "leavemsgs");

            migrationBuilder.RenameColumn(
                name: "Joinmsgs",
                table: "Spclcmds",
                newName: "joinmsgs");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "Spclcmds",
                newName: "guildId");

            migrationBuilder.RenameColumn(
                name: "NameOfBot",
                table: "Spclcmds",
                newName: "prefix");
        }
    }
}
