namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public sealed class StreetNameNameWasCorrectedToClearedAssertions :
        HasStreetNameIdAssertions<StreetNameNameWasCorrectedToCleared, StreetNameNameWasCorrectedToClearedAssertions>
    {
        public StreetNameNameWasCorrectedToClearedAssertions(StreetNameNameWasCorrectedToCleared subject) : base(subject)
        {
        }

        public AndConstraint<StreetNameNameWasCorrectedToClearedAssertions> HaveLanguage(Language? language)
        {
            AssertingThat($"language is {language}");

            Subject.Language.Should().Be(language);

            return And();
        }
    }
}
