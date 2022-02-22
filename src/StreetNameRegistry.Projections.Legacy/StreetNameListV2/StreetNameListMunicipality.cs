namespace StreetNameRegistry.Projections.Legacy.StreetNameListV2
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Municipality;

    public class StreetNameListMunicipality
    {
        public Guid MunicipalityId { get; set; }
        public string NisCode { get; set; }
        public Language? PrimaryLanguage { get; set; }
    }

    public class StreetNameListMunicipalityConfiguration : IEntityTypeConfiguration<StreetNameListMunicipality>
    {
        internal const string TableName = "StreetNameListMunicipality";

        public void Configure(EntityTypeBuilder<StreetNameListMunicipality> builder)
        {
            builder.ToTable(TableName, Schema.Legacy)
                .HasKey(x => x.MunicipalityId)
                .IsClustered();

            builder.Property(x => x.PrimaryLanguage);

            builder.Property(x => x.NisCode);
        }
    }
}
