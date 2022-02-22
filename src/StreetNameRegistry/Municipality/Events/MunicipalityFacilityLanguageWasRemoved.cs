namespace StreetNameRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("MunicipalityFacilitiesLanguageWasRemoved")]
    [EventDescription("Er werd een faciliteitentaal verwijderd van de gemeente.")]
    public class MunicipalityFacilityLanguageWasRemoved : IHasMunicipalityId, IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public Language Language { get; }
        public ProvenanceData Provenance { get; private set; }
        
        public MunicipalityFacilityLanguageWasRemoved(MunicipalityId municipalityId, Language language)
        {
            MunicipalityId = municipalityId;
            Language = language;
        }

        [JsonConstructor]
        private MunicipalityFacilityLanguageWasRemoved(
            Guid municipalityId,
            Language language,
            ProvenanceData provenance)
            : this(new MunicipalityId(municipalityId), language)
            => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        public void SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
