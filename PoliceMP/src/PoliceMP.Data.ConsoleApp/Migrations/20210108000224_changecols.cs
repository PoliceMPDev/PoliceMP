using Microsoft.EntityFrameworkCore.Migrations;

namespace PoliceMP.Data.ConsoleApp.Migrations
{
    public partial class changecols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discord",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "License",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LiveId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Xbl",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Discord",
                table: "Sessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "License",
                table: "Sessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LiveId",
                table: "Sessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Xbl",
                table: "Sessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discord",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "License",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "LiveId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Xbl",
                table: "Sessions");

            migrationBuilder.AddColumn<string>(
                name: "Discord",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "License",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LiveId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Xbl",
                table: "Users",
                nullable: true);
        }
    }
}
