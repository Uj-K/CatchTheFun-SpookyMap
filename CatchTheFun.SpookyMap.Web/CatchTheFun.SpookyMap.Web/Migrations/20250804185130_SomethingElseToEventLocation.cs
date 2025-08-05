using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatchTheFun.SpookyMap.Web.Migrations
{
    /// <inheritdoc />
    public partial class SomethingElseToEventLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SomethingElse",
                table: "EventLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SomethingElse",
                table: "EventLocations");
        }
    }
}
