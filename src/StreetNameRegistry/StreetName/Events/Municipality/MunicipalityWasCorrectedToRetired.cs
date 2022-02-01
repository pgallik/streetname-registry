namespace StreetNameRegistry.StreetName.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("MunicipalityWasCorrectedToRetired")]
    [EventDescription("De gemeente werd gecorrigeerd tot gehistoreerd.")]
    public class MunicipalityWasCorrectedToRetired : IHasMunicipalityId, IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityWasCorrectedToRetired(MunicipalityId municipalityId)
        {
            MunicipalityId = municipalityId;
        }

        [JsonConstructor]
        private MunicipalityWasCorrectedToRetired(
            Guid municipalityId,
            ProvenanceData provenance)
            : this(new MunicipalityId(municipalityId))
            => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());


        public void SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
