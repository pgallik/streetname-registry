namespace StreetNameRegistry
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using NodaTime;

    public class RetirementDate : InstantValueObject<RetirementDate>
    {
        public RetirementDate(Instant instant)
            : base(instant)
        { }
    }
}
