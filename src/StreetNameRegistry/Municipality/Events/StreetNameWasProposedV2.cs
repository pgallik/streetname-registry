namespace StreetNameRegistry.Municipality.Events
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("StreetNameWasProposedV2")]
    [EventDescription("De straatnaam werd voorgesteld.")]
    public class StreetNameWasProposedV2 : IHasMunicipalityId, IHasProvenance, ISetProvenance
    {
        [EventPropertyDescription("Interne GUID van de gemeente.")]
        public Guid MunicipalityId { get; }

        public string NisCode { get; }

        public List<StreetNameName> StreetNameNames { get; }

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
    }
}
