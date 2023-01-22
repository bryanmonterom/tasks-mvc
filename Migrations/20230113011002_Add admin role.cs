using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksMVC.Migrations
{
    /// <inheritdoc />
    public partial class Addadminrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
  IF NOT EXISTS  (SELECT ID FROM AspNetRoles WHERE Name = 'Administrator')
begin
  INSERT INTO AspNetRoles (ID, Name, NormalizedName) VALUES (1,'Administrator','ADMINISTRATOR')
  end");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    DELETE FROM AspNetRoles WHERE Name = 'Administrator'");
        }
    }
}
