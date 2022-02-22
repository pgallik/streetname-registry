namespace StreetNameRegistry.Tests.AggregateTests.WhenNamingMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands;
    using Municipality.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityWasNotAlreadyNamed : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;
        private readonly MunicipalityStreamId _streamId;

        public GivenMunicipalityWasNotAlreadyNamed(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
            _streamId = Fixture.Create<MunicipalityStreamId>();
        }


        [Theory]
        [InlineData(Language.Dutch)]
        [InlineData(Language.French)]
        [InlineData(Language.English)]
        [InlineData(Language.German)]
        public void ThenMunicipalityGetsNamed(Language language)
        {
            var commandNameMunicipality = Fixture.Create<NameMunicipality>().WithName("GreatName", language);
            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                })
                .When(commandNameMunicipality)
                .Then(new[]
                {
                    new Fact(_streamId, new MunicipalityWasNamed(_municipalityId, new MunicipalityName("GreatName", language)))
                }));
        }
    }
}
