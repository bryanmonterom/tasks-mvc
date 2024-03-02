using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksMVC.Migrations
{
    /// <inheritdoc />
    public partial class correctrelationshipuserfieldnme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdUser",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
