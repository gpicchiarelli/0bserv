﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _0bserv.Migrations
{
    /// <inheritdoc />
    public partial class AggiuntaIndiceUnivocoSuUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "RssFeeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_RssFeeds", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "FeedContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RssFeedId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_FeedContents", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_FeedContents_RssFeeds_RssFeedId",
                        column: x => x.RssFeedId,
                        principalTable: "RssFeeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_FeedContents_PublishDate",
                table: "FeedContents",
                column: "PublishDate");

            _ = migrationBuilder.CreateIndex(
                name: "IX_FeedContents_RssFeedId",
                table: "FeedContents",
                column: "RssFeedId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_RssFeeds_Url",
                table: "RssFeeds",
                column: "Url",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "FeedContents");

            _ = migrationBuilder.DropTable(
                name: "RssFeeds");
        }
    }
}
