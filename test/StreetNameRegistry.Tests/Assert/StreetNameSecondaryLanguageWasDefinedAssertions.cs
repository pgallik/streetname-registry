namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public class StreetNameSecondaryLanguageWasDefinedAssertions :
        HasStreetNameIdAssertions<StreetNameSecondaryLanguageWasDefined, StreetNameSecondaryLanguageWasDefinedAssertions>
    {
        public StreetNameSecondaryLanguageWasDefinedAssertions(StreetNameSecondaryLanguageWasDefined subject) : base(subject)
        {
        }

        public AndConstraint<StreetNameSecondaryLanguageWasDefinedAssertions> HaveSecondaryLanguage(Language? secondaryLanguage)
        {
            AssertingThat($"the secondary language is {secondaryLanguage}");

            Subject.SecondaryLanguage.Should().Be(secondaryLanguage);

            return And();
        }
    }
}
