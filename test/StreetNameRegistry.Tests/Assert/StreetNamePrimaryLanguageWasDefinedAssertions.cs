namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public sealed class StreetNamePrimaryLanguageWasDefinedAssertions :
        HasStreetNameIdAssertions<StreetNamePrimaryLanguageWasDefined, StreetNamePrimaryLanguageWasDefinedAssertions>
    {
        public StreetNamePrimaryLanguageWasDefinedAssertions(StreetNamePrimaryLanguageWasDefined subject) : base(subject)
        {
        }

        public AndConstraint<StreetNamePrimaryLanguageWasDefinedAssertions> HavePrimaryLanguage(Language? primaryLanguage)
        {
            AssertingThat($"the primary language is {primaryLanguage}");

            Subject.PrimaryLanguage.Should().Be(primaryLanguage);

            return And();
        }
    }
}
