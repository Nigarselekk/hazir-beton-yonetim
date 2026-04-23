using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HazirBeton.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class M4_AddCancelledBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CancelledById",
                table: "concrete_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_concrete_requests_CancelledById",
                table: "concrete_requests",
                column: "CancelledById");

            migrationBuilder.AddForeignKey(
                name: "FK_concrete_requests_users_CancelledById",
                table: "concrete_requests",
                column: "CancelledById",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_concrete_requests_users_CancelledById",
                table: "concrete_requests");

            migrationBuilder.DropIndex(
                name: "IX_concrete_requests_CancelledById",
                table: "concrete_requests");

            migrationBuilder.DropColumn(
                name: "CancelledById",
                table: "concrete_requests");
        }
    }
}
