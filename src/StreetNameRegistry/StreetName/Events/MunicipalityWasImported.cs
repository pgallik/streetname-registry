namespace StreetNameRegistry.StreetName.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Newtonsoft.Json;

    [EventTags(EventTag.For.Sync)]
    [EventName("MunicipalityWasImported")]
    [EventDescription("De gemeente werd geimporteerd.")]
    public class MunicipalityWasImported : IHasMunicipalityId
    {
        public Guid MunicipalityId { get; }

        public MunicipalityWasImported(MunicipalityId municipalityId)
        {
            MunicipalityId = municipalityId;
        }

        [JsonConstructor]
        private MunicipalityWasImported(
            Guid municipalityId) : this(new MunicipalityId(municipalityId))
        {
            
        }

    }
}
