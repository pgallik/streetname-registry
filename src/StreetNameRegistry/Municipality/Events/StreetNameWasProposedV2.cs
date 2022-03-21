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
    [EventDescription("De straatnaam werd voorgesteld.")]
    public class StreetNameWasProposedV2 : IMunicipalityEvent
    {
        public const string EventName = "StreetNameWasProposedV2"; // BE CAREFUL CHANGING THIS!!

        [EventPropertyDescription("Interne GUID van de gemeente aan dewelke de straatnaam is toegewezen.")]
        public Guid MunicipalityId { get; }

        [EventPropertyDescription("NIS-code (= objectidentificator) van de gemeente aan dewelke de straatnaam is toegewezen.")]
        public string NisCode { get; }

        [EventPropertyDescription("De straatnamen in de officiële en (eventuele) faciliteitentaal van de gemeente. Mogelijkheden: Dutch, French, German of English.")]
        public List<StreetNameName> StreetNameNames { get; }

        [EventPropertyDescription("Objectidentificator van de straatnaam.")]
        public int PersistentLocalId { get; }

        [EventPropertyDescription("Metadata bij het event.")]
        public ProvenanceData Provenance { get; private set; }

        public StreetNameWasProposedV2(
            MunicipalityId municipalityId,
            NisCode nisCode,
            Names streetNameNames,
            PersistentLocalId persistentLocalId)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            StreetNameNames = streetNameNames;
            PersistentLocalId = persistentLocalId;
        }

        [JsonConstructor]
        private StreetNameWasProposedV2(
            Guid municipalityId,
            string nisCode,
            List<StreetNameName> streetNameNames,
            int persistentLocalId,
            ProvenanceData provenance
        ) :
            this(
                new MunicipalityId(municipalityId),
                new NisCode(nisCode),
                new Names(streetNameNames),
                new PersistentLocalId(persistentLocalId))
        => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);

        public IEnumerable<string> GetHashFields()
        {
            var fields = Provenance.GetHashFields().ToList();
            fields.Add(MunicipalityId.ToString("D"));
            fields.Add(NisCode);
            fields.Add(PersistentLocalId.ToString());
            fields.AddRange(StreetNameNames.Select(streetNameName => streetNameName.ToString()));
            return fields;
        }

        public string GetHash() => this.ToHash(EventName);
    }
}
