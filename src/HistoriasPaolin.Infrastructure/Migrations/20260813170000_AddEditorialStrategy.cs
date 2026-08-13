using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoriasPaolin.Infrastructure.Migrations;

public partial class AddEditorialStrategy : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EditorialStrategies",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                Objective = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                PrimaryAudience = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                AgeFrom = table.Column<int>(type: "int", nullable: false),
                AgeTo = table.Column<int>(type: "int", nullable: false),
                PrimaryLanguage = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                SecondaryLanguage = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                Country = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                Tone = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                EducationalApproach = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                ContentStyle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                StorytellingStyle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                DefaultEpisodeDurationSeconds = table.Column<int>(type: "int", nullable: false),
                MinimumEpisodeDurationSeconds = table.Column<int>(type: "int", nullable: false),
                MaximumEpisodeDurationSeconds = table.Column<int>(type: "int", nullable: false),
                ScenesMin = table.Column<int>(type: "int", nullable: false),
                ScenesMax = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                Version = table.Column<int>(type: "int", nullable: false),
                EffectiveFromUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                EffectiveToUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EditorialStrategies", x => x.Id);
                table.ForeignKey("FK_EditorialStrategies_Channels_ChannelId", x => x.ChannelId, "Channels", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "EditorialPillars",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EditorialStrategyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                Weight = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                SortOrder = table.Column<int>(type: "int", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EditorialPillars", x => x.Id);
                table.ForeignKey("FK_EditorialPillars_EditorialStrategies_EditorialStrategyId", x => x.EditorialStrategyId, "EditorialStrategies", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "EditorialTopics",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EditorialStrategyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                Category = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Priority = table.Column<int>(type: "int", nullable: false),
                MinAge = table.Column<int>(type: "int", nullable: false),
                MaxAge = table.Column<int>(type: "int", nullable: false),
                IsAllowed = table.Column<bool>(type: "bit", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EditorialTopics", x => x.Id);
                table.ForeignKey("FK_EditorialTopics_EditorialStrategies_EditorialStrategyId", x => x.EditorialStrategyId, "EditorialStrategies", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "EditorialRestrictions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EditorialStrategyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RestrictionType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                Severity = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                IsBlocking = table.Column<bool>(type: "bit", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EditorialRestrictions", x => x.Id);
                table.ForeignKey("FK_EditorialRestrictions_EditorialStrategies_EditorialStrategyId", x => x.EditorialStrategyId, "EditorialStrategies", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_EditorialStrategies_ChannelId", "EditorialStrategies", "ChannelId");
        migrationBuilder.CreateIndex("IX_EditorialStrategies_IsActive", "EditorialStrategies", "IsActive");
        migrationBuilder.CreateIndex("IX_EditorialStrategies_EffectiveFromUtc", "EditorialStrategies", "EffectiveFromUtc");
        migrationBuilder.CreateIndex("UX_EditorialStrategies_ChannelId_IsActive", "EditorialStrategies", ["ChannelId", "IsActive"], unique: true, filter: "[IsActive] = 1");
        migrationBuilder.CreateIndex("UX_EditorialPillars_StrategyId_Code", "EditorialPillars", ["EditorialStrategyId", "Code"], unique: true);
        migrationBuilder.CreateIndex("UX_EditorialTopics_StrategyId_Code", "EditorialTopics", ["EditorialStrategyId", "Code"], unique: true);
        migrationBuilder.CreateIndex("IX_EditorialRestrictions_StrategyId_Code", "EditorialRestrictions", ["EditorialStrategyId", "Code"]);

        migrationBuilder.Sql("""
DECLARE @ChannelId uniqueidentifier = (SELECT TOP 1 Id FROM Channels WHERE Code = N'historias-paolin');
DECLARE @StrategyId uniqueidentifier = '33333333-3333-4333-8333-333333333333';
IF @ChannelId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM EditorialStrategies WHERE Id = @StrategyId)
BEGIN
    INSERT INTO EditorialStrategies (Id, ChannelId, Name, Description, Objective, PrimaryAudience, AgeFrom, AgeTo, PrimaryLanguage, SecondaryLanguage, Country, Tone, EducationalApproach, ContentStyle, StorytellingStyle, DefaultEpisodeDurationSeconds, MinimumEpisodeDurationSeconds, MaximumEpisodeDurationSeconds, ScenesMin, ScenesMax, IsActive, Version, EffectiveFromUtc, EffectiveToUtc, CreatedAtUtc, CreatedBy)
    VALUES (@StrategyId, @ChannelId, N'Estrategia Editorial Inicial', N'Estrategia base para Historias de Paolín.', N'Crear historias infantiles originales, educativas, seguras y entretenidas.', N'Niños de 2 a 6 años y sus familias.', 2, 6, N'es', N'', N'EC', N'cálido, alegre, claro y respetuoso', N'aprendizaje mediante historias, repetición moderada y participación', N'aventuras breves, visuales y educativas', N'inicio claro, pequeño reto, exploración, resolución y aprendizaje', 90, 60, 180, 5, 10, 1, 1, SYSUTCDATETIME(), NULL, SYSUTCDATETIME(), N'migration');

    INSERT INTO EditorialPillars (Id, EditorialStrategyId, Code, Name, Description, Weight, IsActive, SortOrder, CreatedAtUtc, CreatedBy)
    VALUES
    (NEWID(), @StrategyId, N'aprendizaje-basico', N'Aprendizaje básico', N'Conceptos iniciales para primera infancia.', 25, 1, 1, SYSUTCDATETIME(), N'migration'),
    (NEWID(), @StrategyId, N'emociones-convivencia', N'Emociones y convivencia', N'Reconocer emociones y convivir con respeto.', 20, 1, 2, SYSUTCDATETIME(), N'migration'),
    (NEWID(), @StrategyId, N'creatividad-imaginacion', N'Creatividad e imaginación', N'Juego creativo e imaginación segura.', 20, 1, 3, SYSUTCDATETIME(), N'migration'),
    (NEWID(), @StrategyId, N'habitos-saludables', N'Hábitos saludables', N'Rutinas de cuidado personal.', 15, 1, 4, SYSUTCDATETIME(), N'migration'),
    (NEWID(), @StrategyId, N'naturaleza-entorno', N'Naturaleza y entorno', N'Cuidado y observación del entorno.', 10, 1, 5, SYSUTCDATETIME(), N'migration'),
    (NEWID(), @StrategyId, N'ciencia-preescolar', N'Ciencia preescolar', N'Curiosidad científica temprana.', 10, 1, 6, SYSUTCDATETIME(), N'migration');

    INSERT INTO EditorialTopics (Id, EditorialStrategyId, Code, Name, Description, Category, Priority, MinAge, MaxAge, IsAllowed, IsActive, CreatedAtUtc, CreatedBy)
    SELECT NEWID(), @StrategyId, value, value, N'Tema permitido inicial.', N'general', ROW_NUMBER() OVER (ORDER BY value), 2, 6, 1, 1, SYSUTCDATETIME(), N'migration'
    FROM STRING_SPLIT(N'colores,numeros,formas,animales,emociones,amistad,familia,higiene,alimentacion-saludable,cuidado-del-planeta,seguridad-basica,musica,imaginacion,ciencia-preescolar,estaciones,rutinas,empatia', N',');

    INSERT INTO EditorialRestrictions (Id, EditorialStrategyId, RestrictionType, Code, Description, Severity, IsBlocking, IsActive, CreatedAtUtc, CreatedBy)
    SELECT NEWID(), @StrategyId, N'SafetyRule', value, N'Restricción blocking inicial.', N'block', 1, 1, SYSUTCDATETIME(), N'migration'
    FROM STRING_SPLIT(N'violencia-grafica,armas,miedo-intenso,retos-peligrosos,apuestas,alcohol,drogas,contenido-sexual,marcas-comerciales-eje,personajes-protegidos-terceros,celebridades,clonacion-voz-terceros,canciones-protegidas,ocultar-secretos-padres,pedir-datos-personales-nino', N',');
END
""");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "EditorialRestrictions");
        migrationBuilder.DropTable(name: "EditorialTopics");
        migrationBuilder.DropTable(name: "EditorialPillars");
        migrationBuilder.DropTable(name: "EditorialStrategies");
    }
}
