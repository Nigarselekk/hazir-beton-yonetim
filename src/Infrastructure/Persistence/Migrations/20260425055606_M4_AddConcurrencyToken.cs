using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HazirBeton.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class M4_AddConcurrencyToken : Migration
    {
        // ConcreteRequest now uses PostgreSQL's xmin system column as an optimistic
        // concurrency token. xmin already exists on every row — no DDL needed.
        // The model snapshot still records the mapping so future migrations diff correctly.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
