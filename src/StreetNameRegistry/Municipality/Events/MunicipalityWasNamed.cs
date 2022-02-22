namespace StreetNameRegistry.Municipality.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("MunicipalityWasNamed")]
    [EventDescription("De gemeente werd benoemd.")]
    public class MunicipalityWasNamed : IHasMunicipalityId, IHasProvenance, ISetProvenance
    {
        public Guid MunicipalityId { get; }
        public string Name { get; }
        public Language Language { get; set; }
        public ProvenanceData Provenance { get; private set; }

        public MunicipalityWasNamed(MunicipalityId municipalityId, MunicipalityName name)
        {
            MunicipalityId = municipalityId;
            Name = name.Name;
            Language = name.Language;
        }

        [JsonConstructor]
        private MunicipalityWasNamed(
            Guid municipalityId,
            string name,
            Language language,
            ProvenanceData provenance)
            : this(new MunicipalityId(municipalityId), new MunicipalityName(name, language))
            => ((ISetProvenance)this).SetProvenance(provenance.ToProvenance());

        public void SetProvenance(Provenance provenance) => Provenance = new ProvenanceData(provenance);
    }
}
