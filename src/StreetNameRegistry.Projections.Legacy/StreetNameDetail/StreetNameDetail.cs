namespace StreetNameRegistry.Projections.Legacy.StreetNameDetail
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NodaTime;
    using StreetName;

    public class StreetNameDetail
    {
        public static string VersionTimestampBackingPropertyName = nameof(VersionTimestampAsDateTimeOffset);

        public int? PersistentLocalId { get; set; }
        public Guid StreetNameId { get; set; }

        public string? NisCode { get; set; }

        public string? NameDutch { get; set; }
        public string? NameFrench { get; set; }
        public string? NameGerman { get; set; }
        public string? NameEnglish { get; set; }

        public string? HomonymAdditionDutch { get; set; }
        public string? HomonymAdditionFrench { get; set; }
        public string? HomonymAdditionGerman { get; set; }
        public string? HomonymAdditionEnglish { get; set; }

        public StreetNameStatus? Status { get; set; }

        public bool Complete { get; set; }
        public bool Removed { get; set; }

        private DateTimeOffset VersionTimestampAsDateTimeOffset { get; set; }

        public Instant VersionTimestamp
        {
            get => Instant.FromDateTimeOffset(VersionTimestampAsDateTimeOffset);
            set => VersionTimestampAsDateTimeOffset = value.ToDateTimeOffset();
        }
    }

    public class StreetNameDetailConfiguration : IEntityTypeConfiguration<StreetNameDetail>
    {
        internal const string TableName = "StreetNameDetails";

        public void Configure(EntityTypeBuilder<StreetNameDetail> builder)
        {
            builder.ToTable(TableName, Schema.Legacy)
                .HasKey(x => x.StreetNameId)
                .IsClustered(false);

            builder.Property(x => x.PersistentLocalId);

            builder.Property(StreetNameDetail.VersionTimestampBackingPropertyName)
                .HasColumnName("VersionTimestamp");

            builder.Ignore(x => x.VersionTimestamp);
            builder.Property(x => x.NisCode);

            builder.Property(x => x.NameDutch);
            builder.Property(x => x.NameFrench);
            builder.Property(x => x.NameGerman);
            builder.Property(x => x.NameEnglish);

            builder.Property(x => x.HomonymAdditionDutch);
            builder.Property(x => x.HomonymAdditionFrench);
            builder.Property(x => x.HomonymAdditionGerman);
            builder.Property(x => x.HomonymAdditionEnglish);

            builder.Property(x => x.Status);

            builder.Property(x => x.Complete);

            builder.Property(x => x.Removed);

            builder.HasIndex(p => p.PersistentLocalId)
                .IsClustered();

            builder.HasIndex(p => p.PersistentLocalId)
                .IsUnique()
                .HasFilter($"([{nameof(StreetNameDetail.PersistentLocalId)}] IS NOT NULL)")
                .HasDatabaseName($"IX_StreetNameDetails_PersistentLocalId_1")
                .IsClustered(false);

            builder.HasIndex(x => x.Removed);
        }
    }
}
