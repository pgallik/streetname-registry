namespace StreetNameRegistry.Consumer.Municipality
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using StreetNameRegistry.Infrastructure;
    using System;

    public class MunicipalityConsumerItem
    {
        public Guid MunicipalityId { get; set; }
        public string? NisCode { get; set; }
    }

    public class MunicipalityConsumerItemConfiguration : IEntityTypeConfiguration<MunicipalityConsumerItem>
    {
        private const string TableName = "MunicipalityConsumer";

        public void Configure(EntityTypeBuilder<MunicipalityConsumerItem> builder)
        {
            builder.ToTable(TableName, Schema.Consumer)
                .HasKey(x => x.MunicipalityId)
                .IsClustered(false);

            builder.Property(x => x.NisCode);

            builder.HasIndex(x => x.NisCode).IsClustered();
        }
    }
}
