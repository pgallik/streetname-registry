namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public class StreetNameNameWasCorrectedAssertions :
        HasStreetNameIdAssertions<StreetNameNameWasCorrected, StreetNameNameWasCorrectedAssertions>
    {
        public StreetNameNameWasCorrectedAssertions(StreetNameNameWasCorrected subject) : base(subject)
        {
        }

        public AndConstraint<StreetNameNameWasCorrectedAssertions> HaveName(string name)
        {
            AssertingThat($"the name is {name}");

            Subject.Name.Should().Be(name);

            return And();
        }

        public AndConstraint<StreetNameNameWasCorrectedAssertions> HaveLanguage(Language? language)
        {
            AssertingThat($"language is {language}");

            Subject.Language.Should().Be(language);

            return And();
        }
    }
}
