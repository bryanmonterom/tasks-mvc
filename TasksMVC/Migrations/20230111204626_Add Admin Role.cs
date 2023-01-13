using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS(SELECT ID FROM ASPNETROLES WHERE ID = '150652d5-3b43-4d63-b648-31416307eaee')
	                                    BEGIN
	                                    Insert AspNetRoles (Id, Name, NormalizedName) 
	                                    VALUES('150652d5-3b43-4d63-b648-31416307eaee', 'Administrator','ADMINISTRATOR')
                                    END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM ASPNETROLES WHERE ID = '150652d5-3b43-4d63-b648-31416307eaee'");
        }
    }
}
