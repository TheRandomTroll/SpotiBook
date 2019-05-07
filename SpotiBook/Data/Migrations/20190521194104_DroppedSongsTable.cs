using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpotiBook.Data.Migrations
{
    public partial class DroppedSongsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Songs_UploadedSongId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Posts_UploadedSongId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UploadedSongId",
                table: "Posts");

            migrationBuilder.AddColumn<byte[]>(
                name: "Mp3",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeUrl",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mp3",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "YoutubeUrl",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "UploadedSongId",
                table: "Posts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PathToMP3 = table.Column<string>(nullable: true),
                    YoutubeUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UploadedSongId",
                table: "Posts",
                column: "UploadedSongId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Songs_UploadedSongId",
                table: "Posts",
                column: "UploadedSongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
