using System;
using HistoriasPaolin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HistoriasPaolin.Infrastructure.Migrations;

[DbContext(typeof(HistoriasPaolinDbContext))]
partial class HistoriasPaolinDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.0");
        modelBuilder.Entity("HistoriasPaolin.Domain.Episodes.Episode", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever();
            b.Property<DateTime>("CreatedAtUtc");
            b.Property<string>("CreatedBy").IsRequired().HasMaxLength(120);
            b.Property<decimal>("EstimatedCostUsd").HasPrecision(18, 4);
            b.Property<byte[]>("RowVersion").IsConcurrencyToken().ValueGeneratedOnAddOrUpdate().HasColumnType("rowversion");
            b.Property<string>("Status").IsRequired().HasMaxLength(40);
            b.Property<string>("Title").IsRequired().HasMaxLength(200);
            b.Property<DateTime?>("UpdatedAtUtc");
            b.Property<string>("UpdatedBy").HasMaxLength(120);
            b.HasKey("Id");
            b.HasIndex("Status").HasDatabaseName("IX_Episodes_Status");
            b.ToTable("Episodes");
        });
    }
}
