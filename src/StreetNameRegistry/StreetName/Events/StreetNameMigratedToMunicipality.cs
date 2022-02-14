namespace StreetNameRegistry.StreetName.Events
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("StreetNameMigratedToMunicipality")]
    [EventDescription("De straatnaam werd gemigreerd naar gemeente.")]
    public class StreetNameMigratedToMunicipality : IHasMunicipalityId, IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public Guid StreetNameId { get; }
        public int PersistentLocalId { get; }
        public StreetNameStatus? Status { get; }
        public Language? PrimaryLanguage { get; }
        public Language? SecondaryLanguage { get; }
        public IDictionary<Language, string> Names { get; }
        public IDictionary<Language, string> HomonymAdditions { get; }
        public bool IsCompleted { get; }
        public bool IsRemoved { get; }
        public ProvenanceData Provenance { get; private set; }
        
        public StreetNameMigratedToMunicipality(
            MunicipalityId municipalityId,
            StreetNameId streetNameId,
            PersistentLocalId persistentLocalId,
            StreetNameStatus? status,
            Language? primaryLanguage,
            Language? secondaryLanguage,
            Names names,
            HomonymAdditions homonymAdditions,
            bool isCompleted,
            bool isRemoved)
        {
            MunicipalityId = municipalityId;
            StreetNameId = streetNameId;
            PersistentLocalId = persistentLocalId;
            Status = status;
            PrimaryLanguage = primaryLanguage;
            SecondaryLanguage = secondaryLanguage;
            Names = names.ToDictionary();
            HomonymAdditions = homonymAdditions.ToDictionary();
            IsCompleted = isCompleted;
            IsRemoved = isRemoved;
        }

        [JsonConstructor]
        private StreetNameMigratedToMunicipality(
            Guid municipalityId,
            Guid streetNameId,
            int persistentLocalId,
            StreetNameStatus? status,
            Language? primaryLanguage,
            Language? secondaryLanguage,
            IDictionary<Language, string> names,
            IDictionary<Language, string> homonymAdditions,
            bool isCompleted,
            bool isRemoved,
            ProvenanceData provenance)
            : this(
                new MunicipalityId(municipalityId),
                new StreetNameId(streetNameId),
                new PersistentLocalId(persistentLocalId),
                status,
                primaryLanguage,
                secondaryLanguage,
                new Names(names),
                new HomonymAdditions(homonymAdditions),
                isCompleted,
                isRemoved)
            => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
