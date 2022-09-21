namespace StreetNameRegistry.Tests.Assert
{
    using StreetName.Events;

    public sealed class StreetNameSecondaryLanguageWasClearedAssertions :
        HasStreetNameIdAssertions<StreetNameSecondaryLanguageWasCleared, StreetNameSecondaryLanguageWasClearedAssertions>
    {
        public StreetNameSecondaryLanguageWasClearedAssertions(StreetNameSecondaryLanguageWasCleared subject) : base(subject)
        {
        }
    }
}
