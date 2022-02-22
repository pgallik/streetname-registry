namespace StreetNameRegistry.Municipality.Events
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;
    using StreetNameRegistry.Municipality;

    [EventTags(EventTag.For.Sync)]
    [EventName("StreetNameWasMigratedToMunicipality")]
    [EventDescription("De straatnaam werd gemigreerd naar gemeente.")]
    public class StreetNameWasMigratedToMunicipality : IHasMunicipalityId, IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public string NisCode { get; }
        public Guid StreetNameId { get; }
        public int PersistentLocalId { get; }
        public StreetNameStatus Status { get; }
        public Language? PrimaryLanguage { get; }
        public Language? SecondaryLanguage { get; }
        public IDictionary<Language, string> Names { get; }
        public IDictionary<Language, string> HomonymAdditions { get; }
        public bool IsCompleted { get; }
        public bool IsRemoved { get; }
        public ProvenanceData Provenance { get; private set; }
        
        public StreetNameWasMigratedToMunicipality(
            MunicipalityId municipalityId,
            NisCode nisCode,
            StreetNameId streetNameId,
            PersistentLocalId persistentLocalId,
            StreetNameStatus status,
            Language? primaryLanguage,
            Language? secondaryLanguage,
            Names names,
            HomonymAdditions homonymAdditions,
            bool isCompleted,
            bool isRemoved)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
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
        private StreetNameWasMigratedToMunicipality(
            Guid municipalityId,
            string nisCode,
            Guid streetNameId,
            int persistentLocalId,
            StreetNameStatus status,
            Language? primaryLanguage,
            Language? secondaryLanguage,
            IDictionary<Language, string> names,
            IDictionary<Language, string> homonymAdditions,
            bool isCompleted,
            bool isRemoved,
            ProvenanceData provenance)
            : this(
                new MunicipalityId(municipalityId),
                new NisCode(nisCode),
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
