using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatchTheFun.SpookyMap.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoAndTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "EventLocations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "EventLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "EventLocations",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "EventLocations");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "EventLocations");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "EventLocations");
        }
    }
}
