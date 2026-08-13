using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoriasPaolin.Infrastructure.Migrations;

public partial class AddChannelManagement : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Channels",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                Language = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                Country = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                TimeZone = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                IsMadeForKids = table.Column<bool>(type: "bit", nullable: false),
                DefaultAspectRatio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                DefaultVideoDurationSeconds = table.Column<int>(type: "int", nullable: false),
                DefaultPublicationPrivacy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Channels", x => x.Id));

        migrationBuilder.CreateTable(
            name: "ChannelBrands",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                LongDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                PrimaryLanguage = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                VisualStyle = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                ToneOfVoice = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                TargetAudience = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                TargetAgeFrom = table.Column<int>(type: "int", nullable: false),
                TargetAgeTo = table.Column<int>(type: "int", nullable: false),
                BrandPrompt = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                CharacterConsistencyPrompt = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                NegativePrompt = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChannelBrands", x => x.Id);
                table.ForeignKey(
                    name: "FK_ChannelBrands_Channels_ChannelId",
                    column: x => x.ChannelId,
                    principalTable: "Channels",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Channels_IsActive", table: "Channels", column: "IsActive");
        migrationBuilder.CreateIndex(name: "IX_Channels_Name", table: "Channels", column: "Name");
        migrationBuilder.CreateIndex(name: "UX_Channels_Code", table: "Channels", column: "Code", unique: true);
        migrationBuilder.CreateIndex(name: "IX_ChannelBrands_ChannelId", table: "ChannelBrands", column: "ChannelId");
        migrationBuilder.CreateIndex(name: "IX_ChannelBrands_IsActive", table: "ChannelBrands", column: "IsActive");
        migrationBuilder.CreateIndex(name: "UX_ChannelBrands_ChannelId_IsActive", table: "ChannelBrands", columns: ["ChannelId", "IsActive"], unique: true, filter: "[IsActive] = 1");

        var channelId = new Guid("11111111-1111-4111-8111-111111111111");
        var brandId = new Guid("22222222-2222-4222-8222-222222222222");
        var createdAt = new DateTime(2026, 8, 13, 0, 0, 0, DateTimeKind.Utc);

        migrationBuilder.InsertData(
            table: "Channels",
            columns: ["Id", "Code", "Name", "Description", "Language", "Country", "TimeZone", "IsActive", "IsMadeForKids", "DefaultAspectRatio", "DefaultVideoDurationSeconds", "DefaultPublicationPrivacy", "CreatedAtUtc", "CreatedBy"],
            values: [channelId, "historias-paolin", "Historias de Paolín", "Canal infantil original para historias educativas y seguras.", "es", "EC", "America/Guayaquil", true, true, "16:9", 180, "private", createdAt, "migration"]);

        migrationBuilder.InsertData(
            table: "ChannelBrands",
            columns: ["Id", "ChannelId", "DisplayName", "ShortDescription", "LongDescription", "PrimaryLanguage", "VisualStyle", "ToneOfVoice", "TargetAudience", "TargetAgeFrom", "TargetAgeTo", "BrandPrompt", "CharacterConsistencyPrompt", "NegativePrompt", "IsActive", "CreatedAtUtc", "CreatedBy"],
            values: [brandId, channelId, "Historias de Paolín", "Historias infantiles originales.", "Historias infantiles educativas, seguras y originales para primera infancia.", "es", "Colorido, amable, limpio y apto para ninos pequenos.", "Calido, curioso, respetuoso y tranquilo.", "Ninos de 2 a 6 anos y sus familias.", 2, 6, "Crear historias originales, tiernas y educativas para primera infancia.", "Mantener personajes consistentes, seguros y sin referencias a franquicias existentes.", "Sin violencia, miedo intenso, marcas, franquicias, imitaciones, canciones protegidas o estilos de personajes existentes.", true, createdAt, "migration"]);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ChannelBrands");
        migrationBuilder.DropTable(name: "Channels");
    }
}
