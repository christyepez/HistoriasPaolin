using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Domain.Common;
using HistoriasPaolin.Domain.Episodes;
using HistoriasPaolin.Domain.Integration;
using HistoriasPaolin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HistoriasPaolin.Tests;

public sealed class PersistenceHardeningTests
{
    [Fact]
    public void ModelSnapshotIncludesAccumulatedEntities()
    {
        using var dbContext = CreateContext();
        var model = dbContext.Model;

        Assert.NotNull(model.FindEntityType(typeof(Channel)));
        Assert.NotNull(model.FindEntityType(typeof(ChannelBrand)));
        Assert.NotNull(model.FindEntityType(typeof(EditorialStrategy)));
        Assert.NotNull(model.FindEntityType(typeof(EditorialPillar)));
        Assert.NotNull(model.FindEntityType(typeof(EditorialTopic)));
        Assert.NotNull(model.FindEntityType(typeof(EditorialRestriction)));
        Assert.NotNull(model.FindEntityType(typeof(EpisodeScene)));
        Assert.NotNull(model.FindEntityType(typeof(OutboxMessage)));
    }

    [Fact]
    public void MigrationsAreInExpectedOrder()
    {
        using var dbContext = CreateContext();
        var migrations = dbContext.GetService<IMigrationsAssembly>().Migrations.Keys.ToList();

        Assert.Equal(
            [
                "20260803220000_InitialSqlServerSchema",
                "20260813160000_AddChannelManagement",
                "20260813170000_AddEditorialStrategy",
                "20260813201219_HP103PersistenceHardening"
            ],
            migrations);
    }

    [Fact]
    public void ChannelCodeIsUnique()
    {
        using var dbContext = CreateContext();
        var channel = dbContext.Model.FindEntityType(typeof(Channel))!;
        var index = channel.GetIndexes().Single(index => index.GetDatabaseName() == "UX_Channels_Code");

        Assert.True(index.IsUnique);
    }

    [Fact]
    public void OnlyOneActiveBrandPerChannelIsEnforced()
    {
        using var dbContext = CreateContext();
        var brand = dbContext.Model.FindEntityType(typeof(ChannelBrand))!;
        var index = brand.GetIndexes().Single(index => index.GetDatabaseName() == "UX_ChannelBrands_ChannelId_IsActive");

        Assert.True(index.IsUnique);
        Assert.Equal("[IsActive] = 1", index.GetFilter());
    }

    [Fact]
    public void OnlyOneActiveEditorialStrategyPerChannelIsEnforced()
    {
        using var dbContext = CreateContext();
        var strategy = dbContext.Model.FindEntityType(typeof(EditorialStrategy))!;
        var index = strategy.GetIndexes().Single(index => index.GetDatabaseName() == "UX_EditorialStrategies_ChannelId_IsActive");

        Assert.True(index.IsUnique);
        Assert.Equal("[IsActive] = 1", index.GetFilter());
    }

    [Fact]
    public void PillarAndTopicCodesAreUniquePerStrategy()
    {
        using var dbContext = CreateContext();
        var pillar = dbContext.Model.FindEntityType(typeof(EditorialPillar))!;
        var topic = dbContext.Model.FindEntityType(typeof(EditorialTopic))!;

        Assert.True(pillar.GetIndexes().Single(index => index.GetDatabaseName() == "UX_EditorialPillars_StrategyId_Code").IsUnique);
        Assert.True(topic.GetIndexes().Single(index => index.GetDatabaseName() == "UX_EditorialTopics_StrategyId_Code").IsUnique);
    }

    [Fact]
    public void MutableEntitiesUseRowVersion()
    {
        using var dbContext = CreateContext();
        var auditableTypes = new[]
        {
            typeof(Channel), typeof(ChannelBrand), typeof(EditorialStrategy), typeof(EditorialPillar), typeof(EditorialTopic), typeof(EditorialRestriction),
            typeof(Episode), typeof(EpisodeScene), typeof(OutboxMessage)
        };

        foreach (var type in auditableTypes)
        {
            Assert.True(typeof(AuditableEntity).IsAssignableFrom(type));
            var rowVersion = dbContext.Model.FindEntityType(type)!.FindProperty(nameof(AuditableEntity.RowVersion))!;
            Assert.True(rowVersion.IsConcurrencyToken);
            Assert.Equal("rowversion", rowVersion.GetColumnType());
        }
    }

    [Fact]
    public void ForeignKeysUseExplicitCascadeDeleteBehavior()
    {
        using var dbContext = CreateContext();
        var fkNames = dbContext.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetForeignKeys())
            .Select(fk => new { Name = fk.GetConstraintName(), fk.DeleteBehavior })
            .ToDictionary(fk => fk.Name!, fk => fk.DeleteBehavior);

        Assert.Equal(DeleteBehavior.Cascade, fkNames["FK_ChannelBrands_Channels_ChannelId"]);
        Assert.Equal(DeleteBehavior.Cascade, fkNames["FK_EditorialStrategies_Channels_ChannelId"]);
        Assert.Equal(DeleteBehavior.Cascade, fkNames["FK_EditorialPillars_EditorialStrategies_EditorialStrategyId"]);
        Assert.Equal(DeleteBehavior.Cascade, fkNames["FK_EditorialTopics_EditorialStrategies_EditorialStrategyId"]);
        Assert.Equal(DeleteBehavior.Cascade, fkNames["FK_EditorialRestrictions_EditorialStrategies_EditorialStrategyId"]);
        Assert.Equal(DeleteBehavior.Cascade, fkNames["FK_EpisodeScenes_Episodes_EpisodeId"]);
    }

    [Fact]
    public void SeedsAreDeterministicAndIdempotent()
    {
        var channelMigration = File.ReadAllText(Path.Combine(MigrationDirectory(), "20260813160000_AddChannelManagement.cs"));
        var editorialMigration = File.ReadAllText(Path.Combine(MigrationDirectory(), "20260813170000_AddEditorialStrategy.cs"));

        Assert.Contains("IF NOT EXISTS", channelMigration);
        Assert.Contains("IF @ChannelId IS NOT NULL AND NOT EXISTS", editorialMigration);
        Assert.DoesNotContain("NEWID()", editorialMigration);
        Assert.DoesNotContain("SYSUTCDATETIME()", editorialMigration);
    }

    [Fact]
    public void Hp103MigrationIsEmptyModelHardeningMigration()
    {
        var migration = File.ReadAllText(Directory.GetFiles(MigrationDirectory(), "*HP103PersistenceHardening.cs").Single(file => !file.EndsWith(".Designer.cs", StringComparison.Ordinal)));

        Assert.Contains("public partial class HP103PersistenceHardening", migration);
        Assert.DoesNotContain("CreateTable", migration);
        Assert.DoesNotContain("DropTable", migration);
        Assert.DoesNotContain("AlterColumn", migration);
    }

    private static HistoriasPaolinDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<HistoriasPaolinDbContext>()
            .UseSqlServer("Server=localhost,14333;Database=HistoriasPaolinDb;Integrated Security=true;TrustServerCertificate=true")
            .Options;

        return new HistoriasPaolinDbContext(options);
    }

    private static string MigrationDirectory()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null && !Directory.Exists(Path.Combine(current.FullName, "src", "HistoriasPaolin.Infrastructure", "Migrations")))
        {
            current = current.Parent;
        }

        Assert.NotNull(current);
        return Path.Combine(current!.FullName, "src", "HistoriasPaolin.Infrastructure", "Migrations");
    }
}
