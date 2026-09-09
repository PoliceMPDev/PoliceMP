using Microsoft.EntityFrameworkCore.Migrations;

namespace PoliceMP.Data.ConsoleApp.Migrations
{
    public partial class AddDisconnectReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisconnectedReason",
                table: "Sessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisconnectedReason",
                table: "Sessions");
        }
    }
}
