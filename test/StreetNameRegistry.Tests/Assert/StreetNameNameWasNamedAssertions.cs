namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public sealed class StreetNameNameWasNamedAssertions :
        HasStreetNameIdAssertions<StreetNameWasNamed, StreetNameNameWasNamedAssertions>
    {
        public StreetNameNameWasNamedAssertions(StreetNameWasNamed subject) : base(subject)
        {
        }

        public AndConstraint<StreetNameNameWasNamedAssertions> HaveName(string name)
        {
            AssertingThat($"the name is {name}");

            Subject.Name.Should().Be(name);

            return And();
        }

        public AndConstraint<StreetNameNameWasNamedAssertions> HaveLanguage(Language? language)
        {
            AssertingThat($"language is {language}");

            Subject.Language.Should().Be(language);

            return And();
        }
    }
}
