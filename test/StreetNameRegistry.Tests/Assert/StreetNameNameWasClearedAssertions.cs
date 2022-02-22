namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public class StreetNameNameWasClearedAssertions :
        HasStreetNameIdAssertions<StreetNameNameWasCleared, StreetNameNameWasClearedAssertions>
    {
        public StreetNameNameWasClearedAssertions(StreetNameNameWasCleared subject) : base(subject)
        {
        }

        public AndConstraint<StreetNameNameWasClearedAssertions> HaveLanguage(Language? language)
        {
            AssertingThat($"language is {language}");

            Subject.Language.Should().Be(language);

            return And();
        }
    }
}
