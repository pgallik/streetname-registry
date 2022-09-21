namespace StreetNameRegistry.Tests.Assert
{
    using StreetName.Events;

    public static class StreetNameBecameCompleteAssertionsProvider
    {
        public static StreetNameBecameCompleteAssertions Should(this StreetNameBecameComplete subject)
        {
            return new StreetNameBecameCompleteAssertions(subject);
        }
    }

    public sealed class StreetNameBecameCompleteAssertions :
        HasStreetNameIdAssertions<StreetNameBecameComplete, StreetNameBecameCompleteAssertions>
    {
        public StreetNameBecameCompleteAssertions(StreetNameBecameComplete subject) : base(subject)
        {
        }
    }
}

