namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public class StreetNameSecondaryLanguageWasCorrectedAssertions :
        HasStreetNameIdAssertions<StreetNameSecondaryLanguageWasCorrected, StreetNameSecondaryLanguageWasCorrectedAssertions>
    {
        public StreetNameSecondaryLanguageWasCorrectedAssertions(StreetNameSecondaryLanguageWasCorrected subject) : base(subject)
        {
        }

        public AndConstraint<StreetNameSecondaryLanguageWasCorrectedAssertions> HaveSecondaryLanguage(Language? secondaryLanguage)
        {
            AssertingThat($"the secondary language is {secondaryLanguage}");

            Subject.SecondaryLanguage.Should().Be(secondaryLanguage);

            return And();
        }
    }

    public class StreetNameSecondaryLanguageWasCorrectedToClearedAssertions :
        HasStreetNameIdAssertions<StreetNameSecondaryLanguageWasCorrectedToCleared, StreetNameSecondaryLanguageWasCorrectedToClearedAssertions>
    {
        public StreetNameSecondaryLanguageWasCorrectedToClearedAssertions(StreetNameSecondaryLanguageWasCorrectedToCleared subject) : base(subject)
        {
        }
    }
}
