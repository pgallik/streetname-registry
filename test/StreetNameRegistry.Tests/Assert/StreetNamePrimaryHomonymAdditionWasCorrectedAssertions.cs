namespace StreetNameRegistry.Tests.Assert
{
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;

    public class StreetNameHomonymAdditionWasCorrectedAssertions :
        HasStreetNameIdAssertions<StreetNameHomonymAdditionWasCorrected, StreetNameHomonymAdditionWasCorrectedAssertions>
    {
        public StreetNameHomonymAdditionWasCorrectedAssertions(StreetNameHomonymAdditionWasCorrected subject) : base(subject)
        {
        }

        public AndConstraint<StreetNameHomonymAdditionWasCorrectedAssertions> HaveHomonymAddition(string homonymAddition)
        {
            AssertingThat($"the homonym addition is {homonymAddition}");

            Subject.HomonymAddition.Should().Be(homonymAddition);

            return And();
        }

        public AndConstraint<StreetNameHomonymAdditionWasCorrectedAssertions> HaveLanguage(Language? language)
        {
            AssertingThat($"the language is {language}");

            Subject.Language.Should().Be(language);

            return And();
        }
    }

    public class StreetNameHomonymAdditionWasCorrectedToClearedAssertions :
        HasStreetNameIdAssertions<StreetNameHomonymAdditionWasCorrectedToCleared, StreetNameHomonymAdditionWasCorrectedToClearedAssertions>
    {
        public StreetNameHomonymAdditionWasCorrectedToClearedAssertions(StreetNameHomonymAdditionWasCorrectedToCleared subject) : base(subject)
        {
        }

        public AndConstraint<StreetNameHomonymAdditionWasCorrectedToClearedAssertions> HaveLanguage(Language? language)
        {
            AssertingThat($"the language is {language}");

            Subject.Language.Should().Be(language);

            return And();
        }
    }
}
