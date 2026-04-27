using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HazirBeton.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class M5_AddSmsOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sms_logs_ConcreteRequestId",
                table: "sms_logs");

            migrationBuilder.DropColumn(
                name: "IsSuccessful",
                table: "sms_logs");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAttemptAt",
                table: "sms_logs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastErrorMessage",
                table: "sms_logs",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextAttemptAt",
                table: "sms_logs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderMessageId",
                table: "sms_logs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "sms_logs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "sms_logs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_sms_logs_ConcreteRequestId_EventType",
                table: "sms_logs",
                columns: new[] { "ConcreteRequestId", "EventType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sms_logs_Status_NextAttemptAt",
                table: "sms_logs",
                columns: new[] { "Status", "NextAttemptAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sms_logs_ConcreteRequestId_EventType",
                table: "sms_logs");

            migrationBuilder.DropIndex(
                name: "IX_sms_logs_Status_NextAttemptAt",
                table: "sms_logs");

            migrationBuilder.DropColumn(
                name: "LastAttemptAt",
                table: "sms_logs");

            migrationBuilder.DropColumn(
                name: "LastErrorMessage",
                table: "sms_logs");

            migrationBuilder.DropColumn(
                name: "NextAttemptAt",
                table: "sms_logs");

            migrationBuilder.DropColumn(
                name: "ProviderMessageId",
                table: "sms_logs");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "sms_logs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "sms_logs");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccessful",
                table: "sms_logs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_sms_logs_ConcreteRequestId",
                table: "sms_logs",
                column: "ConcreteRequestId");
        }
    }
}
