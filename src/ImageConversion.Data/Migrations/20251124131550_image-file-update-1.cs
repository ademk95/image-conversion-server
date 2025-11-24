using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageConversion.Data.Migrations
{
    /// <inheritdoc />
    public partial class imagefileupdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAt",
                table: "ImageFiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessStatus",
                table: "ImageFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishedAt",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "ProcessStatus",
                table: "ImageFiles");
        }
    }
}
