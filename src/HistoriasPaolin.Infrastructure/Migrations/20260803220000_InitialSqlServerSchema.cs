using System;
using HistoriasPaolin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoriasPaolin.Infrastructure.Migrations;

[DbContext(typeof(HistoriasPaolinDbContext))]
[Migration("20260803220000_InitialSqlServerSchema")]
public partial class InitialSqlServerSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Episodes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                EstimatedCostUsd = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Episodes", x => x.Id));

        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                AggregateType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                AggregateId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                EventType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                HeadersJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CorrelationId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                CausationId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                IdempotencyKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                AttemptCount = table.Column<int>(type: "int", nullable: false),
                LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                OccurredAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                ProcessedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_OutboxMessages", x => x.Id));

        migrationBuilder.CreateTable(
            name: "EpisodeScenes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EpisodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SortOrder = table.Column<int>(type: "int", nullable: false),
                Prompt = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EpisodeScenes", x => x.Id);
                table.ForeignKey(
                    name: "FK_EpisodeScenes_Episodes_EpisodeId",
                    column: x => x.EpisodeId,
                    principalTable: "Episodes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Episodes_Status", table: "Episodes", column: "Status");
        migrationBuilder.CreateIndex(name: "IX_OutboxMessages_Status_OccurredAtUtc", table: "OutboxMessages", columns: ["Status", "OccurredAtUtc"]);
        migrationBuilder.CreateIndex(name: "UX_OutboxMessages_TenantId_IdempotencyKey", table: "OutboxMessages", columns: ["TenantId", "IdempotencyKey"], unique: true, filter: "[IdempotencyKey] IS NOT NULL");
        migrationBuilder.CreateIndex(name: "UX_EpisodeScenes_EpisodeId_SortOrder", table: "EpisodeScenes", columns: ["EpisodeId", "SortOrder"], unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "EpisodeScenes");
        migrationBuilder.DropTable(name: "OutboxMessages");
        migrationBuilder.DropTable(name: "Episodes");
    }
}
