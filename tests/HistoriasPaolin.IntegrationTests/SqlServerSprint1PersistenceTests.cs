using HistoriasPaolin.Application.Common;
using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Infrastructure.Configuration;
using HistoriasPaolin.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HistoriasPaolin.IntegrationTests;

[Trait("Category", "RequiresSqlServer")]
public sealed class SqlServerSprint1PersistenceTests : IAsyncLifetime
{
    private const string IntegrationDatabaseName = "HistoriasPaolinDb_IntegrationTests";
    private static readonly Guid ChannelId = Guid.Parse("10500000-0000-4000-8000-000000000001");
    private static readonly Guid BrandId = Guid.Parse("10500000-0000-4000-8000-000000000002");
    private static readonly Guid StrategyId = Guid.Parse("10500000-0000-4000-8000-000000000003");
    private string? _skipReason;

    public async Task InitializeAsync()
    {
        if (!await SqlServerIsReachableAsync())
        {
            _skipReason = "SQL Server localhost,14333 is not reachable.";
            return;
        }

        await using var dbContext = CreateContext("sql-test-setup");
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (_skipReason is not null)
        {
            return;
        }

        await using var dbContext = CreateContext("sql-test-cleanup");
        await dbContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task LatestMigrationIsAppliedAndCreatedAtIndexesExist()
    {
        if (!SqlServerAvailable())
        {
            return;
        }

        await using var dbContext = CreateContext("sql-reader");
        var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();
        Assert.Equal("20260907201736_HP104ConcurrencyAuditHardening", appliedMigrations.Last());

        var expected = new Dictionary<string, string>
        {
            ["Channels"] = "IX_Channels_CreatedAtUtc",
            ["ChannelBrands"] = "IX_ChannelBrands_CreatedAtUtc",
            ["EditorialStrategies"] = "IX_EditorialStrategies_CreatedAtUtc",
            ["EditorialPillars"] = "IX_EditorialPillars_CreatedAtUtc",
            ["EditorialTopics"] = "IX_EditorialTopics_CreatedAtUtc",
            ["EditorialRestrictions"] = "IX_EditorialRestrictions_CreatedAtUtc",
            ["Episodes"] = "IX_Episodes_CreatedAtUtc",
            ["EpisodeScenes"] = "IX_EpisodeScenes_CreatedAtUtc",
            ["OutboxMessages"] = "IX_OutboxMessages_CreatedAtUtc"
        };

        await using var connection = new SqlConnection(BuildConnectionString(IntegrationDatabaseName));
        await connection.OpenAsync();
        foreach (var (tableName, indexName) in expected)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT COUNT(1)
                FROM sys.indexes i
                INNER JOIN sys.tables t ON i.object_id = t.object_id
                INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                WHERE t.name = @tableName AND i.name = @indexName AND c.name = N'CreatedAtUtc';
                """;
            command.Parameters.AddWithValue("@tableName", tableName);
            command.Parameters.AddWithValue("@indexName", indexName);

            Assert.Equal(1, Convert.ToInt32(await command.ExecuteScalarAsync()));
        }
    }

    [Fact]
    public async Task ChannelAndBrandPersistAndReloadFromFreshContext()
    {
        if (!SqlServerAvailable())
        {
            return;
        }

        await using (var dbContext = CreateContext("creator"))
        {
            var channel = ValidChannel();
            channel.SetBrand(ValidBrand());
            dbContext.Channels.Add(channel);
            await dbContext.SaveChangesAsync();

            Assert.NotEmpty(channel.RowVersion);
            Assert.Equal("creator", channel.CreatedBy);
            Assert.Equal("creator", channel.ActiveBrand!.CreatedBy);
        }

        await using (var dbContext = CreateContext("reader"))
        {
            var persisted = await dbContext.Channels
                .AsNoTracking()
                .Include(channel => channel.Brands)
                .SingleAsync(channel => channel.Id == ChannelId);

            Assert.Equal("hp105-channel", persisted.Code);
            Assert.Equal("private", persisted.DefaultPublicationPrivacy);
            Assert.NotNull(persisted.ActiveBrand);
            Assert.Equal("Prompt de marca HP-105", persisted.ActiveBrand!.BrandPrompt);
            Assert.Equal("Consistencia de personajes HP-105", persisted.ActiveBrand.CharacterConsistencyPrompt);
            Assert.Equal("Sin marcas ni imitaciones", persisted.ActiveBrand.NegativePrompt);
            Assert.NotEmpty(persisted.RowVersion);
            Assert.NotEmpty(persisted.ActiveBrand.RowVersion);
        }
    }

    [Fact]
    public async Task EditorialStrategyPersistsAndReloadsFromFreshContext()
    {
        if (!SqlServerAvailable())
        {
            return;
        }

        await using (var dbContext = CreateContext("creator"))
        {
            var channel = ValidChannel();
            var strategy = ValidStrategy();
            strategy.Pillars.Add(new EditorialPillar { Id = Guid.Parse("10500000-0000-4000-8000-000000000004"), Code = "aprendizaje", Name = "Aprendizaje", Description = "Aprender jugando", Weight = 60, IsActive = true, SortOrder = 1 });
            strategy.Topics.Add(new EditorialTopic { Id = Guid.Parse("10500000-0000-4000-8000-000000000005"), Code = "colores", Name = "Colores", Description = "Colores basicos", Category = "general", Priority = 1, MinAge = 2, MaxAge = 6, IsAllowed = true, IsActive = true });
            strategy.Restrictions.Add(new EditorialRestriction { Id = Guid.Parse("10500000-0000-4000-8000-000000000006"), RestrictionType = "SafetyRule", Code = "marcas", Description = "Sin marcas", Severity = "block", IsBlocking = true, IsActive = true });
            channel.EditorialStrategies.Add(strategy);
            dbContext.Channels.Add(channel);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = CreateContext("reader"))
        {
            var persisted = await dbContext.EditorialStrategies
                .AsNoTracking()
                .Include(strategy => strategy.Channel)
                .Include(strategy => strategy.Pillars)
                .Include(strategy => strategy.Topics)
                .Include(strategy => strategy.Restrictions)
                .SingleAsync(strategy => strategy.Id == StrategyId);

            Assert.Equal(ChannelId, persisted.ChannelId);
            Assert.Equal(1, persisted.Version);
            Assert.True(persisted.IsActive);
            Assert.Equal(new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc), persisted.EffectiveFromUtc);
            Assert.Null(persisted.EffectiveToUtc);
            Assert.Single(persisted.Pillars);
            Assert.Single(persisted.Topics);
            Assert.Single(persisted.Restrictions);
            Assert.NotEmpty(persisted.RowVersion);
            Assert.Equal("creator", persisted.CreatedBy);
        }
    }

    [Fact]
    public async Task UpdatePreservesCreatedAuditAndChangesRowVersion()
    {
        if (!SqlServerAvailable())
        {
            return;
        }

        await using (var dbContext = CreateContext("creator"))
        {
            dbContext.Channels.Add(ValidChannel());
            await dbContext.SaveChangesAsync();
        }

        byte[] originalRowVersion;
        DateTime createdAt;
        await using (var dbContext = CreateContext("reader"))
        {
            var channel = await dbContext.Channels.AsNoTracking().SingleAsync(channel => channel.Id == ChannelId);
            originalRowVersion = channel.RowVersion;
            createdAt = channel.CreatedAtUtc;
            Assert.Equal("creator", channel.CreatedBy);
        }

        await using (var dbContext = CreateContext("editor"))
        {
            var channel = await dbContext.Channels.SingleAsync(channel => channel.Id == ChannelId);
            channel.Name = "HP-105 actualizado";
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = CreateContext("reader"))
        {
            var channel = await dbContext.Channels.AsNoTracking().SingleAsync(channel => channel.Id == ChannelId);
            Assert.Equal(createdAt, channel.CreatedAtUtc);
            Assert.Equal("creator", channel.CreatedBy);
            Assert.NotNull(channel.UpdatedAtUtc);
            Assert.Equal("editor", channel.UpdatedBy);
            Assert.NotEqual(originalRowVersion, channel.RowVersion);
        }
    }

    [Fact]
    public async Task StaleRowVersionProducesConcurrencyConflictAndPreservesCommittedState()
    {
        if (!SqlServerAvailable())
        {
            return;
        }

        await using (var dbContext = CreateContext("creator"))
        {
            dbContext.Channels.Add(ValidChannel());
            await dbContext.SaveChangesAsync();
        }

        await using var first = CreateContext("first-editor");
        await using var second = CreateContext("second-editor");
        var firstChannel = await first.Channels.SingleAsync(channel => channel.Id == ChannelId);
        var secondChannel = await second.Channels.SingleAsync(channel => channel.Id == ChannelId);

        firstChannel.Name = "Primer cambio";
        await first.SaveChangesAsync();

        secondChannel.Name = "Cambio obsoleto";
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());

        await using var reader = CreateContext("reader");
        var persisted = await reader.Channels.AsNoTracking().SingleAsync(channel => channel.Id == ChannelId);
        Assert.Equal("Primer cambio", persisted.Name);
    }

    private bool SqlServerAvailable()
    {
        // xUnit v2 has no first-class dynamic skip. The explicit SQL availability
        // command in HP-105 reports BLOCKED_ENVIRONMENT when this guard is active.
        return _skipReason is null;
    }

    private static async Task<bool> SqlServerIsReachableAsync()
    {
        try
        {
            await using var connection = new SqlConnection(BuildConnectionString("master"));
            await connection.OpenAsync();
            return true;
        }
        catch (SqlException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static HistoriasPaolinDbContext CreateContext(string userName)
    {
        var options = new DbContextOptionsBuilder<HistoriasPaolinDbContext>()
            .UseSqlServer(BuildConnectionString(IntegrationDatabaseName), sql =>
            {
                sql.MigrationsAssembly(typeof(HistoriasPaolinDbContext).Assembly.FullName);
                sql.CommandTimeout(30);
            })
            .AddInterceptors(new AuditingSaveChangesInterceptor(new TestCurrentUser(userName)))
            .Options;

        return new HistoriasPaolinDbContext(options);
    }

    private static string BuildConnectionString(string database)
    {
        var options = new SqlServerOptions
        {
            Host = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? "localhost",
            Port = int.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_PORT"), out var port) ? port : 14333,
            Instance = Environment.GetEnvironmentVariable("SQLSERVER_INSTANCE"),
            Database = database,
            User = Environment.GetEnvironmentVariable("SQLSERVER_USER"),
            Password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD"),
            IntegratedSecurity = bool.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_INTEGRATED_SECURITY"), out var integratedSecurity) && integratedSecurity,
            Encrypt = bool.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_ENCRYPT"), out var encrypt) ? encrypt : true,
            TrustServerCertificate = !bool.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_TRUST_SERVER_CERTIFICATE"), out var trustServerCertificate) || trustServerCertificate,
            ConnectTimeout = 3,
            CommandTimeout = 30
        };

        return SqlServerConnectionStringFactory.Create(options);
    }

    private static Channel ValidChannel() => new()
    {
        Id = ChannelId,
        Code = "hp105-channel",
        Name = "HP-105 Channel",
        Description = "Canal sintetico de pruebas HP-105",
        Language = "es",
        Country = "EC",
        TimeZone = "America/Guayaquil",
        IsMadeForKids = true,
        DefaultAspectRatio = "16:9",
        DefaultVideoDurationSeconds = 180,
        DefaultPublicationPrivacy = "private"
    };

    private static ChannelBrand ValidBrand() => new()
    {
        Id = BrandId,
        ChannelId = ChannelId,
        DisplayName = "Marca HP-105",
        ShortDescription = "Marca de prueba",
        LongDescription = "Marca sintetica para pruebas de integracion SQL",
        PrimaryLanguage = "es",
        VisualStyle = "Colorido y limpio",
        ToneOfVoice = "Calido",
        TargetAudience = "Ninos y familias",
        TargetAgeFrom = 2,
        TargetAgeTo = 6,
        BrandPrompt = "Prompt de marca HP-105",
        CharacterConsistencyPrompt = "Consistencia de personajes HP-105",
        NegativePrompt = "Sin marcas ni imitaciones",
        IsActive = true
    };

    private static EditorialStrategy ValidStrategy() => new()
    {
        Id = StrategyId,
        ChannelId = ChannelId,
        Name = "Estrategia HP-105",
        Description = "Estrategia sintetica",
        Objective = "Validar persistencia SQL Server",
        PrimaryAudience = "Ninos y familias",
        AgeFrom = 2,
        AgeTo = 6,
        PrimaryLanguage = "es",
        SecondaryLanguage = "",
        Country = "EC",
        Tone = "Calido",
        EducationalApproach = "Aprender jugando",
        ContentStyle = "Aventuras breves",
        StorytellingStyle = "Inicio, reto y cierre",
        DefaultEpisodeDurationSeconds = 90,
        MinimumEpisodeDurationSeconds = 60,
        MaximumEpisodeDurationSeconds = 180,
        ScenesMin = 5,
        ScenesMax = 10,
        IsActive = true,
        EffectiveFromUtc = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc)
    };

    private sealed class TestCurrentUser(string userName) : ICurrentUser
    {
        public string UserName { get; } = userName;
    }
}
