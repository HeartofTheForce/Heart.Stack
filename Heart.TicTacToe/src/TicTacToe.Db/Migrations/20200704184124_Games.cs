using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TicTacToe.Db.Migrations
{
    public partial class Games : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerOne = table.Column<string>(nullable: true),
                    PlayerTwo = table.Column<string>(nullable: true),
                    BoardHistory = table.Column<int>(nullable: false),
                    GameResult = table.Column<string>(nullable: true),
                    GameState = table.Column<string>(nullable: false),
                    LastActionDate = table.Column<DateTimeOffset>(nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatchmakingEntries",
                columns: table => new
                {
                    PlayerId = table.Column<string>(nullable: false),
                    LatestJoinDate = table.Column<DateTimeOffset>(nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchmakingEntries", x => x.PlayerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "MatchmakingEntries");
        }
    }
}
