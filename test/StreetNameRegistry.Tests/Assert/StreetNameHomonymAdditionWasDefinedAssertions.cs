namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public class StreetNameHomonymAdditionWasDefinedAssertions :
        HasStreetNameIdAssertions<StreetNameHomonymAdditionWasDefined, StreetNameHomonymAdditionWasDefinedAssertions>
    {
        public StreetNameHomonymAdditionWasDefinedAssertions(StreetNameHomonymAdditionWasDefined subject) : base(subject)
        {
        }

        public AndConstraint<StreetNameHomonymAdditionWasDefinedAssertions> HaveHomonymAddition(string homonymAddition)
        {
            AssertingThat($"the primary homonym addition is {homonymAddition}");

            Subject.HomonymAddition.Should().Be(homonymAddition);

            return And();
        }

        public AndConstraint<StreetNameHomonymAdditionWasDefinedAssertions> HaveLanguage(Language language)
        {
            AssertingThat($"the language is {language}");

            Subject.Language.Should().Be(language);

            return And();
        }
    }
}
