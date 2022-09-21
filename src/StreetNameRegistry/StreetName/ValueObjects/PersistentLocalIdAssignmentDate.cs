namespace StreetNameRegistry.StreetName
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;
    using NodaTime;

    public sealed class PersistentLocalIdAssignmentDate : InstantValueObject<PersistentLocalIdAssignmentDate>
    {
        public PersistentLocalIdAssignmentDate([JsonProperty("value")] Instant assignmentDate) : base(assignmentDate) { }
    }
}
