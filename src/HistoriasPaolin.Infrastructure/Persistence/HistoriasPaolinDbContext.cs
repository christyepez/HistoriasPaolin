using HistoriasPaolin.Domain.Common;
using HistoriasPaolin.Domain.Episodes;
using HistoriasPaolin.Domain.Integration;
using Microsoft.EntityFrameworkCore;

namespace HistoriasPaolin.Infrastructure.Persistence;

public sealed class HistoriasPaolinDbContext(DbContextOptions<HistoriasPaolinDbContext> options) : DbContext(options)
{
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<EpisodeScene> EpisodeScenes => Set<EpisodeScene>();
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
