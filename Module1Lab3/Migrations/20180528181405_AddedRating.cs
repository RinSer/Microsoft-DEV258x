using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MovieApp.Migrations
{
    public partial class AddedRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "film_rating_index",
                table: "film");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "film");

            migrationBuilder.AddColumn<int>(
                name: "RatingId",
                table: "film",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    RatingCode = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.RatingId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_film_RatingId",
                table: "film",
                column: "RatingId");

            migrationBuilder.AddForeignKey(
                name: "FK_film_Rating_RatingId",
                table: "film",
                column: "RatingId",
                principalTable: "Rating",
                principalColumn: "RatingId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_film_Rating_RatingId",
                table: "film");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_film_RatingId",
                table: "film");

            migrationBuilder.DropColumn(
                name: "RatingId",
                table: "film");

            migrationBuilder.AddColumn<string>(
                name: "Rating",
                table: "film",
                maxLength: 45,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "film_rating_index",
                table: "film",
                column: "Rating");
        }
    }
}
