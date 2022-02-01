namespace StreetNameRegistry.Tests.AggregateTests.WhenAddingMunicipalityFacilityLanguage
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using StreetName.Commands.Municipality;
    using StreetName.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityHasNoFacilityLanguage : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;

        public GivenMunicipalityHasNoFacilityLanguage(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
        }

        [Theory]
        [InlineData(Language.Dutch)]
        [InlineData(Language.French)]
        [InlineData(Language.English)]
        [InlineData(Language.German)]
        public void ThenNone(Language language)
        {
            Fixture.Register(() => language);
            var commandLanguageAdded = Fixture.Create<AddFacilityLanguageToMunicipality>();
            Assert(new Scenario()
                .Given(_municipalityId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                })
                .When(commandLanguageAdded)
                .Then(new[]
                {
                    new Fact(_municipalityId, new MunicipalityFacilityLanguageWasAdded(_municipalityId, language))
                }));
        }
    }
}
