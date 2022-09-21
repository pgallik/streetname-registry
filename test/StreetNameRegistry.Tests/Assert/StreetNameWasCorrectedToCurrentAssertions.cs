namespace StreetNameRegistry.Tests.Assert
{
    using StreetName.Events;

    public sealed class StreetNameWasCorrectedToCurrentAssertions :
        HasStreetNameIdAssertions<StreetNameWasCorrectedToCurrent, StreetNameWasCorrectedToCurrentAssertions>
    {
        public StreetNameWasCorrectedToCurrentAssertions(StreetNameWasCorrectedToCurrent subject) : base(subject)
        {
        }
    }
}
