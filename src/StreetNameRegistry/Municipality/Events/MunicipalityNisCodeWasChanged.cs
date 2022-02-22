namespace StreetNameRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("MunicipalityNisCodeWasChanged")]
    [EventDescription("De NisCode van de gemeente werd gewijzigd.")]
    public class MunicipalityNisCodeWasChanged: IHasMunicipalityId, IHasProvenance, ISetProvenance
    {
        [EventPropertyDescription("Interne GUID van de straatnaam.")]
        public Guid MunicipalityId { get; }
        public string NisCode { get; }

        [EventPropertyDescription("Metadata bij het event.")]
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityNisCodeWasChanged(
            MunicipalityId municipalityId,
            NisCode nisCode)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
        }

        [JsonConstructor]
        private MunicipalityNisCodeWasChanged(
            Guid municipalityId,
            string nisCode,
            ProvenanceData provenance)
            : this(
                new MunicipalityId(municipalityId),
                new NisCode(nisCode))
            => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        void ISetProvenance.SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
