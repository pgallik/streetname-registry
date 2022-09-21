namespace StreetNameRegistry.Tests.Assert
{
    using StreetName.Events;

    public sealed class StreetNameWasRetiredAssertions :
        HasStreetNameIdAssertions<StreetNameWasRetired, StreetNameWasRetiredAssertions>
    {
        public StreetNameWasRetiredAssertions(StreetNameWasRetired subject) : base(subject)
        {
        }
    }

    public sealed class StreetNameWasCorrectedToRetiredAssertions :
        HasStreetNameIdAssertions<StreetNameWasCorrectedToRetired, StreetNameWasCorrectedToRetiredAssertions>
    {
        public StreetNameWasCorrectedToRetiredAssertions(StreetNameWasCorrectedToRetired subject) : base(subject)
        {
        }
    }
}
