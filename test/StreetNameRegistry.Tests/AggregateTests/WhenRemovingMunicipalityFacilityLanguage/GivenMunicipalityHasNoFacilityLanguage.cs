namespace StreetNameRegistry.Tests.AggregateTests.WhenRemovingMunicipalityFacilityLanguage
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands;
    using Municipality.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityHasNoFacilityLanguage : StreetNameRegistryTest
    {
        private readonly MunicipalityStreamId _streamId;

        public GivenMunicipalityHasNoFacilityLanguage(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _streamId = Fixture.Create<MunicipalityStreamId>();
        }

        [Theory]
        [InlineData(Language.Dutch)]
        [InlineData(Language.French)]
        [InlineData(Language.English)]
        [InlineData(Language.German)]
        public void ThenNone(Language language)
        {
            Fixture.Register(() => language);
            var commandLanguageRemoved = Fixture.Create<RemoveFacilityLanguageFromMunicipality>();
            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                })
                .When(commandLanguageRemoved)
                .ThenNone());
        }
    }
}
