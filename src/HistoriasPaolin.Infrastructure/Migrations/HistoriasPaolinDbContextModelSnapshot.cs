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

        modelBuilder.Entity("HistoriasPaolin.Domain.Channels.Channel", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever();
            b.Property<string>("Code").IsRequired().HasMaxLength(100);
            b.Property<string>("Country").IsRequired().HasMaxLength(12);
            b.Property<DateTime>("CreatedAtUtc");
            b.Property<string>("CreatedBy").IsRequired().HasMaxLength(120);
            b.Property<string>("DefaultAspectRatio").IsRequired().HasMaxLength(20);
            b.Property<string>("DefaultPublicationPrivacy").IsRequired().HasMaxLength(20);
            b.Property<int>("DefaultVideoDurationSeconds");
            b.Property<string>("Description").IsRequired().HasMaxLength(1000);
            b.Property<bool>("IsActive");
            b.Property<bool>("IsMadeForKids");
            b.Property<string>("Language").IsRequired().HasMaxLength(12);
            b.Property<string>("Name").IsRequired().HasMaxLength(200);
            b.Property<byte[]>("RowVersion").IsConcurrencyToken().ValueGeneratedOnAddOrUpdate().HasColumnType("rowversion");
            b.Property<string>("TimeZone").IsRequired().HasMaxLength(120);
            b.Property<DateTime?>("UpdatedAtUtc");
            b.Property<string>("UpdatedBy").HasMaxLength(120);
            b.HasKey("Id");
            b.HasIndex("IsActive").HasDatabaseName("IX_Channels_IsActive");
            b.HasIndex("Name").HasDatabaseName("IX_Channels_Name");
            b.HasIndex("Code").IsUnique().HasDatabaseName("UX_Channels_Code");
            b.ToTable("Channels");
        });

        modelBuilder.Entity("HistoriasPaolin.Domain.Channels.ChannelBrand", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever();
            b.Property<string>("BrandPrompt").IsRequired().HasMaxLength(4000);
            b.Property<Guid>("ChannelId");
            b.Property<string>("CharacterConsistencyPrompt").IsRequired().HasMaxLength(4000);
            b.Property<DateTime>("CreatedAtUtc");
            b.Property<string>("CreatedBy").IsRequired().HasMaxLength(120);
            b.Property<string>("DisplayName").IsRequired().HasMaxLength(200);
            b.Property<bool>("IsActive");
            b.Property<string>("LongDescription").IsRequired().HasMaxLength(2000);
            b.Property<string>("NegativePrompt").IsRequired().HasMaxLength(4000);
            b.Property<string>("PrimaryLanguage").IsRequired().HasMaxLength(12);
            b.Property<byte[]>("RowVersion").IsConcurrencyToken().ValueGeneratedOnAddOrUpdate().HasColumnType("rowversion");
            b.Property<string>("ShortDescription").IsRequired().HasMaxLength(500);
            b.Property<int>("TargetAgeFrom");
            b.Property<int>("TargetAgeTo");
            b.Property<string>("TargetAudience").IsRequired().HasMaxLength(1000);
            b.Property<string>("ToneOfVoice").IsRequired().HasMaxLength(1000);
            b.Property<DateTime?>("UpdatedAtUtc");
            b.Property<string>("UpdatedBy").HasMaxLength(120);
            b.Property<string>("VisualStyle").IsRequired().HasMaxLength(1000);
            b.HasKey("Id");
            b.HasIndex("ChannelId").HasDatabaseName("IX_ChannelBrands_ChannelId");
            b.HasIndex("IsActive").HasDatabaseName("IX_ChannelBrands_IsActive");
            b.HasIndex("ChannelId", "IsActive").IsUnique().HasFilter("[IsActive] = 1").HasDatabaseName("UX_ChannelBrands_ChannelId_IsActive");
            b.ToTable("ChannelBrands");
        });

        modelBuilder.Entity("HistoriasPaolin.Domain.Channels.ChannelBrand", b =>
        {
            b.HasOne("HistoriasPaolin.Domain.Channels.Channel", "Channel")
                .WithMany("Brands")
                .HasForeignKey("ChannelId")
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ChannelBrands_Channels_ChannelId");

            b.Navigation("Channel");
        });

        modelBuilder.Entity("HistoriasPaolin.Domain.Channels.Channel", b =>
        {
            b.Navigation("Brands");
        });
    }
}
