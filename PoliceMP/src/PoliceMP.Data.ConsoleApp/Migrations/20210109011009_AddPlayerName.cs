using Microsoft.EntityFrameworkCore.Migrations;

namespace PoliceMP.Data.ConsoleApp.Migrations
{
    public partial class AddPlayerName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayerName",
                table: "Sessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerName",
                table: "Sessions");
        }
    }
}
