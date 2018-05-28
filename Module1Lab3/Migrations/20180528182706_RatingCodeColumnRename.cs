using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MovieApp.Migrations
{
    public partial class RatingCodeColumnRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingCode",
                table: "Rating");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Rating",
                type: "longtext",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Rating");

            migrationBuilder.AddColumn<string>(
                name: "RatingCode",
                table: "Rating",
                nullable: false,
                defaultValue: "");
        }
    }
}
