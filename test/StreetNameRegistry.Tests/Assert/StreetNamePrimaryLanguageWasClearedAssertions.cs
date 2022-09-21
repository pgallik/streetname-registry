namespace StreetNameRegistry.Tests.Assert
{
    using StreetName.Events;

    public sealed class StreetNamePrimaryLanguageWasClearedAssertions :
        HasStreetNameIdAssertions<StreetNamePrimaryLanguageWasCleared, StreetNamePrimaryLanguageWasClearedAssertions>
    {
        public StreetNamePrimaryLanguageWasClearedAssertions(StreetNamePrimaryLanguageWasCleared subject) : base(subject)
        {
        }
    }
}
