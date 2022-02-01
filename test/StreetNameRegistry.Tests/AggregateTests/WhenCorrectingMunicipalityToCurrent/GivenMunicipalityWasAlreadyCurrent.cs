namespace StreetNameRegistry.Tests.AggregateTests.WhenCorrectingMunicipalityToCurrent
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

    public class GivenMunicipalityWasAlreadyCurrent : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;

        public GivenMunicipalityWasAlreadyCurrent(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
        }

        [Fact]
        public void ThenNone()
        {
            var commandCorrectMunicipality = Fixture.Create<CorrectToCurrentMunicipality>();
            Assert(new Scenario()
                .Given(_municipalityId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityBecameCurrent>()
                })
                .When(commandCorrectMunicipality)
                .ThenNone());
        }

        [Fact]
        public void AndChangedToRetired_ThenItIsChangedToCurrent()
        {
            var commandCorrectMunicipality = Fixture.Create<CorrectToCurrentMunicipality>();
            Assert(new Scenario()
                .Given(_municipalityId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityWasRetired>()
                })
                .When(commandCorrectMunicipality)
                .Then(new[]
                {
                    new Fact(_municipalityId, new MunicipalityWasCorrectedToCurrent(_municipalityId))
                }));
        }
    }
}
