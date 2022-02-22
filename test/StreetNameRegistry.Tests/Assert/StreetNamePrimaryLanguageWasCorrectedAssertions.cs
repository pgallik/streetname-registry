namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public class StreetNamePrimaryLanguageWasCorrectedAssertions :
        HasStreetNameIdAssertions<StreetNamePrimaryLanguageWasCorrected, StreetNamePrimaryLanguageWasCorrectedAssertions>
    {
        public StreetNamePrimaryLanguageWasCorrectedAssertions(StreetNamePrimaryLanguageWasCorrected subject) : base(subject)
        {
        }

        public AndConstraint<StreetNamePrimaryLanguageWasCorrectedAssertions> HavePrimaryLanguage(Language? primaryLanguage)
        {
            AssertingThat($"the primary language is {primaryLanguage}");

            Subject.PrimaryLanguage.Should().Be(primaryLanguage);

            return And();
        }
    }

    public class StreetNamePrimaryLanguageWasCorrectedToClearedAssertions :
        HasStreetNameIdAssertions<StreetNamePrimaryLanguageWasCorrectedToCleared, StreetNamePrimaryLanguageWasCorrectedToClearedAssertions>
    {
        public StreetNamePrimaryLanguageWasCorrectedToClearedAssertions(StreetNamePrimaryLanguageWasCorrectedToCleared subject) : base(subject)
        {
        }
    }
}
