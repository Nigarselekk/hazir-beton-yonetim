using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HazirBeton.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConcreteRequestContactPhones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequesterPhone",
                table: "concrete_requests",
                newName: "SiteContactPhone");

            migrationBuilder.AddColumn<string>(
                name: "CompanyContactPhone",
                table: "concrete_requests",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyContactPhone",
                table: "concrete_requests");

            migrationBuilder.RenameColumn(
                name: "SiteContactPhone",
                table: "concrete_requests",
                newName: "RequesterPhone");
        }
    }
}
