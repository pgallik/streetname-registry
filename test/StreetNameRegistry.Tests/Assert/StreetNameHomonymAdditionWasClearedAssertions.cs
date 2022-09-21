namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public sealed class StreetNameHomonymAdditionWasClearedAssertions :
        HasStreetNameIdAssertions<StreetNameHomonymAdditionWasCleared, StreetNameHomonymAdditionWasClearedAssertions>
    {
        public StreetNameHomonymAdditionWasClearedAssertions(StreetNameHomonymAdditionWasCleared subject) : base(subject)
        {
        }

        public AndConstraint<StreetNameHomonymAdditionWasClearedAssertions> HaveLanguage(Language language)
        {
            AssertingThat($"the Language is {language}");

            Subject.Language.Should().Be(language);

            return And();
        }
    }
}
