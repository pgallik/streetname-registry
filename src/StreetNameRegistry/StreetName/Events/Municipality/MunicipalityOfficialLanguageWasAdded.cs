namespace StreetNameRegistry.StreetName.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("MunicipalityOfficalLanguageWasAdded")]
    [EventDescription("Er werd een officiële taal toegevoegd aan de gemeente.")]
    public class MunicipalityOfficialLanguageWasAdded : IHasMunicipalityId, IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public Language Language { get; }
        public ProvenanceData Provenance { get; private set; }
        
        public MunicipalityOfficialLanguageWasAdded(MunicipalityId municipalityId, Language language)
        {
            MunicipalityId = municipalityId;
            Language = language;
        }

        [JsonConstructor]
        private MunicipalityOfficialLanguageWasAdded(
            Guid municipalityId,
            Language language,
            ProvenanceData provenance)
            : this(new MunicipalityId(municipalityId), language)
            => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        public void SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
