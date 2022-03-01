namespace StreetNameRegistry.Municipality.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;
    using StreetNameRegistry.Municipality;

    [EventTags(EventTag.For.Sync)]
    [EventName(EventName)]
    [EventDescription("De straatnaam werd gemigreerd naar gemeente.")]
    public class StreetNameWasMigratedToMunicipality : IMunicipalityEvent
    {
        public const string EventName = "StreetNameWasMigratedToMunicipality"; // BE CAREFUL CHANGING THIS!!

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

        public IEnumerable<string> GetHashFields()
        {
            var fields = Provenance.GetHashFields().ToList();
            fields.Add(MunicipalityId.ToString("D"));
            fields.Add(NisCode);
            fields.Add(StreetNameId.ToString("D"));
            fields.Add(PersistentLocalId.ToString());
            fields.Add(Status.ToString());
            fields.Add(PrimaryLanguage?.ToString() ?? string.Empty);
            fields.Add(SecondaryLanguage?.ToString() ?? string.Empty);
            fields.AddRange(Names.Select(x => x.Key + x.Value));
            fields.AddRange(HomonymAdditions.Select(x => x.Key + x.Value));
            fields.Add(IsCompleted.ToString());
            fields.Add(IsRemoved.ToString());
            return fields;
        }

        public string GetHash() => this.ToHash(EventName);
    }
}
