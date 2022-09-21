namespace StreetNameRegistry.Tests.Assert
{
    using StreetName.Events;

    public sealed class StreetNameWasProposedAssertions :
        HasStreetNameIdAssertions<StreetNameWasProposed, StreetNameWasProposedAssertions>
    {
        public StreetNameWasProposedAssertions(StreetNameWasProposed subject) : base(subject)
        {
        }
    }

    public sealed class StreetNameWasCorrectedToProposedAssertions :
        HasStreetNameIdAssertions<StreetNameWasCorrectedToProposed, StreetNameWasCorrectedToProposedAssertions>
    {
        public StreetNameWasCorrectedToProposedAssertions(StreetNameWasCorrectedToProposed subject) : base(subject)
        {
        }
    }
}
