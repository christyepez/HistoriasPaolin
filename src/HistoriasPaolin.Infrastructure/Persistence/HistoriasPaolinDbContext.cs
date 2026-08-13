using HistoriasPaolin.Domain.Common;
using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Domain.Episodes;
using HistoriasPaolin.Domain.Integration;
using Microsoft.EntityFrameworkCore;

namespace HistoriasPaolin.Infrastructure.Persistence;

public sealed class HistoriasPaolinDbContext(DbContextOptions<HistoriasPaolinDbContext> options) : DbContext(options)
{
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<EpisodeScene> EpisodeScenes => Set<EpisodeScene>();
    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<ChannelBrand> ChannelBrands => Set<ChannelBrand>();
    public DbSet<EditorialStrategy> EditorialStrategies => Set<EditorialStrategy>();
    public DbSet<EditorialPillar> EditorialPillars => Set<EditorialPillar>();
    public DbSet<EditorialTopic> EditorialTopics => Set<EditorialTopic>();
    public DbSet<EditorialRestriction> EditorialRestrictions => Set<EditorialRestriction>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Episode>(entity =>
        {
            entity.ToTable("Episodes");
            ConfigureAuditableEntity(entity);
            entity.Property(x => x.Title).HasColumnName("Title").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Status).HasColumnName("Status").HasMaxLength(40).IsRequired();
            entity.Property(x => x.EstimatedCostUsd).HasColumnName("EstimatedCostUsd").HasPrecision(18, 4);
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_Episodes_Status");
        });

        modelBuilder.Entity<EpisodeScene>(entity =>
        {
            entity.ToTable("EpisodeScenes");
            ConfigureAuditableEntity(entity);
            entity.Property(x => x.EpisodeId).HasColumnName("EpisodeId").IsRequired();
            entity.Property(x => x.SortOrder).HasColumnName("SortOrder").IsRequired();
            entity.Property(x => x.Prompt).HasColumnName("Prompt").HasMaxLength(4000).IsRequired();
            entity.Property(x => x.Status).HasColumnName("Status").HasMaxLength(40).IsRequired();
            entity.HasIndex(x => new { x.EpisodeId, x.SortOrder }).IsUnique().HasDatabaseName("UX_EpisodeScenes_EpisodeId_SortOrder");
            entity.HasOne(x => x.Episode)
                .WithMany(x => x.Scenes)
                .HasForeignKey(x => x.EpisodeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EpisodeScenes_Episodes_EpisodeId");
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            ConfigureAuditableEntity(entity);
            entity.Property(x => x.TenantId).HasMaxLength(80).IsRequired();
            entity.Property(x => x.AggregateType).HasMaxLength(120).IsRequired();
            entity.Property(x => x.AggregateId).HasMaxLength(120).IsRequired();
            entity.Property(x => x.EventType).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PayloadJson).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(x => x.HeadersJson).HasColumnType("nvarchar(max)");
            entity.Property(x => x.CorrelationId).HasMaxLength(120).IsRequired();
            entity.Property(x => x.CausationId).HasMaxLength(120);
            entity.Property(x => x.IdempotencyKey).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(40).IsRequired();
            entity.Property(x => x.LastError).HasMaxLength(2000);
            entity.Property(x => x.OccurredAtUtc).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.IdempotencyKey }).IsUnique().HasFilter("[IdempotencyKey] IS NOT NULL").HasDatabaseName("UX_OutboxMessages_TenantId_IdempotencyKey");
            entity.HasIndex(x => new { x.Status, x.OccurredAtUtc }).HasDatabaseName("IX_OutboxMessages_Status_OccurredAtUtc");
        });

        modelBuilder.Entity<Channel>(entity =>
        {
            entity.ToTable("Channels");
            ConfigureAuditableEntity(entity);
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Language).HasMaxLength(12).IsRequired();
            entity.Property(x => x.Country).HasMaxLength(12).IsRequired();
            entity.Property(x => x.TimeZone).HasMaxLength(120).IsRequired();
            entity.Property(x => x.DefaultAspectRatio).HasMaxLength(20).IsRequired();
            entity.Property(x => x.DefaultPublicationPrivacy).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Channels_Code");
            entity.HasIndex(x => x.Name).HasDatabaseName("IX_Channels_Name");
            entity.HasIndex(x => x.IsActive).HasDatabaseName("IX_Channels_IsActive");
        });

        modelBuilder.Entity<ChannelBrand>(entity =>
        {
            entity.ToTable("ChannelBrands");
            ConfigureAuditableEntity(entity);
            entity.Property(x => x.ChannelId).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ShortDescription).HasMaxLength(500).IsRequired();
            entity.Property(x => x.LongDescription).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.PrimaryLanguage).HasMaxLength(12).IsRequired();
            entity.Property(x => x.VisualStyle).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.ToneOfVoice).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.TargetAudience).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.BrandPrompt).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.CharacterConsistencyPrompt).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.NegativePrompt).HasMaxLength(4000).IsRequired();
            entity.HasIndex(x => x.ChannelId).HasDatabaseName("IX_ChannelBrands_ChannelId");
            entity.HasIndex(x => x.IsActive).HasDatabaseName("IX_ChannelBrands_IsActive");
            entity.HasIndex(x => new { x.ChannelId, x.IsActive })
                .IsUnique()
                .HasFilter("[IsActive] = 1")
                .HasDatabaseName("UX_ChannelBrands_ChannelId_IsActive");
            entity.HasOne(x => x.Channel)
                .WithMany(x => x.Brands)
                .HasForeignKey(x => x.ChannelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ChannelBrands_Channels_ChannelId");
        });

        modelBuilder.Entity<EditorialStrategy>(entity =>
        {
            entity.ToTable("EditorialStrategies");
            ConfigureAuditableEntity(entity);
            entity.Property(x => x.ChannelId).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Objective).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.PrimaryAudience).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.PrimaryLanguage).HasMaxLength(12).IsRequired();
            entity.Property(x => x.SecondaryLanguage).HasMaxLength(12).IsRequired();
            entity.Property(x => x.Country).HasMaxLength(12).IsRequired();
            entity.Property(x => x.Tone).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.EducationalApproach).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.ContentStyle).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.StorytellingStyle).HasMaxLength(2000).IsRequired();
            entity.HasIndex(x => x.ChannelId).HasDatabaseName("IX_EditorialStrategies_ChannelId");
            entity.HasIndex(x => x.IsActive).HasDatabaseName("IX_EditorialStrategies_IsActive");
            entity.HasIndex(x => x.EffectiveFromUtc).HasDatabaseName("IX_EditorialStrategies_EffectiveFromUtc");
            entity.HasIndex(x => new { x.ChannelId, x.IsActive })
                .IsUnique()
                .HasFilter("[IsActive] = 1")
                .HasDatabaseName("UX_EditorialStrategies_ChannelId_IsActive");
            entity.HasOne(x => x.Channel)
                .WithMany(x => x.EditorialStrategies)
                .HasForeignKey(x => x.ChannelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EditorialStrategies_Channels_ChannelId");
        });

        modelBuilder.Entity<EditorialPillar>(entity =>
        {
            entity.ToTable("EditorialPillars");
            ConfigureAuditableEntity(entity);
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            entity.HasIndex(x => new { x.EditorialStrategyId, x.Code }).IsUnique().HasDatabaseName("UX_EditorialPillars_StrategyId_Code");
            entity.HasOne(x => x.EditorialStrategy)
                .WithMany(x => x.Pillars)
                .HasForeignKey(x => x.EditorialStrategyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EditorialPillars_EditorialStrategies_EditorialStrategyId");
        });

        modelBuilder.Entity<EditorialTopic>(entity =>
        {
            entity.ToTable("EditorialTopics");
            ConfigureAuditableEntity(entity);
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Category).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => new { x.EditorialStrategyId, x.Code }).IsUnique().HasDatabaseName("UX_EditorialTopics_StrategyId_Code");
            entity.HasOne(x => x.EditorialStrategy)
                .WithMany(x => x.Topics)
                .HasForeignKey(x => x.EditorialStrategyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EditorialTopics_EditorialStrategies_EditorialStrategyId");
        });

        modelBuilder.Entity<EditorialRestriction>(entity =>
        {
            entity.ToTable("EditorialRestrictions");
            ConfigureAuditableEntity(entity);
            entity.Property(x => x.RestrictionType).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Severity).HasMaxLength(40).IsRequired();
            entity.HasIndex(x => new { x.EditorialStrategyId, x.Code }).HasDatabaseName("IX_EditorialRestrictions_StrategyId_Code");
            entity.HasOne(x => x.EditorialStrategy)
                .WithMany(x => x.Restrictions)
                .HasForeignKey(x => x.EditorialStrategyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EditorialRestrictions_EditorialStrategies_EditorialStrategyId");
        });
    }

    private static void ConfigureAuditableEntity<TEntity>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> entity)
        where TEntity : AuditableEntity
    {
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
        entity.Property(x => x.CreatedAtUtc).HasColumnName("CreatedAtUtc").IsRequired();
        entity.Property(x => x.UpdatedAtUtc).HasColumnName("UpdatedAtUtc");
        entity.Property(x => x.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(120).IsRequired();
        entity.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy").HasMaxLength(120);
        entity.Property(x => x.RowVersion).HasColumnName("RowVersion").IsRowVersion();
    }
}
