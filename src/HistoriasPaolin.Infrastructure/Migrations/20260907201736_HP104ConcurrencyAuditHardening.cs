using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoriasPaolin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HP104ConcurrencyAuditHardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CreatedAtUtc",
                table: "OutboxMessages",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeScenes_CreatedAtUtc",
                table: "EpisodeScenes",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_CreatedAtUtc",
                table: "Episodes",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EditorialTopics_CreatedAtUtc",
                table: "EditorialTopics",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EditorialStrategies_CreatedAtUtc",
                table: "EditorialStrategies",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EditorialRestrictions_CreatedAtUtc",
                table: "EditorialRestrictions",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EditorialPillars_CreatedAtUtc",
                table: "EditorialPillars",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_CreatedAtUtc",
                table: "Channels",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelBrands_CreatedAtUtc",
                table: "ChannelBrands",
                column: "CreatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_CreatedAtUtc",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_EpisodeScenes_CreatedAtUtc",
                table: "EpisodeScenes");

            migrationBuilder.DropIndex(
                name: "IX_Episodes_CreatedAtUtc",
                table: "Episodes");

            migrationBuilder.DropIndex(
                name: "IX_EditorialTopics_CreatedAtUtc",
                table: "EditorialTopics");

            migrationBuilder.DropIndex(
                name: "IX_EditorialStrategies_CreatedAtUtc",
                table: "EditorialStrategies");

            migrationBuilder.DropIndex(
                name: "IX_EditorialRestrictions_CreatedAtUtc",
                table: "EditorialRestrictions");

            migrationBuilder.DropIndex(
                name: "IX_EditorialPillars_CreatedAtUtc",
                table: "EditorialPillars");

            migrationBuilder.DropIndex(
                name: "IX_Channels_CreatedAtUtc",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_ChannelBrands_CreatedAtUtc",
                table: "ChannelBrands");
        }
    }
}
