using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HazirBeton.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConcreteRequestAndVehicleRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_concrete_requests_vehicles_VehicleId",
                table: "concrete_requests");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "concrete_requests",
                newName: "ApprovedById");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "concrete_requests",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "ConcreteType",
                table: "concrete_requests",
                newName: "MaterialType");

            migrationBuilder.RenameIndex(
                name: "IX_concrete_requests_VehicleId",
                table: "concrete_requests",
                newName: "IX_concrete_requests_ApprovedById");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdentityOrTaxNumber",
                table: "customers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "concrete_requests",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "concrete_requests",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "concrete_request_vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcreteRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_concrete_request_vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_concrete_request_vehicles_concrete_requests_ConcreteRequest~",
                        column: x => x.ConcreteRequestId,
                        principalTable: "concrete_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_concrete_request_vehicles_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_concrete_request_vehicles_ConcreteRequestId_VehicleId",
                table: "concrete_request_vehicles",
                columns: new[] { "ConcreteRequestId", "VehicleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_concrete_request_vehicles_VehicleId",
                table: "concrete_request_vehicles",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_concrete_requests_users_ApprovedById",
                table: "concrete_requests",
                column: "ApprovedById",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_concrete_requests_users_ApprovedById",
                table: "concrete_requests");

            migrationBuilder.DropTable(
                name: "concrete_request_vehicles");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "users");

            migrationBuilder.DropColumn(
                name: "IdentityOrTaxNumber",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "concrete_requests");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "concrete_requests");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "concrete_requests",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "MaterialType",
                table: "concrete_requests",
                newName: "ConcreteType");

            migrationBuilder.RenameColumn(
                name: "ApprovedById",
                table: "concrete_requests",
                newName: "VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_concrete_requests_ApprovedById",
                table: "concrete_requests",
                newName: "IX_concrete_requests_VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_concrete_requests_vehicles_VehicleId",
                table: "concrete_requests",
                column: "VehicleId",
                principalTable: "vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
