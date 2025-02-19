namespace StreetNameRegistry.Municipality
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using NodaTime;

    public sealed class PersistentLocalIdAssignmentDate : InstantValueObject<PersistentLocalIdAssignmentDate>
    {
        public PersistentLocalIdAssignmentDate(Instant assignmentDate) : base(assignmentDate) { }
    }
}
