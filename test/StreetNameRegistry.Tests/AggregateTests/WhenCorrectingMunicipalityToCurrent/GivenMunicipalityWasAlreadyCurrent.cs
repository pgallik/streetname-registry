namespace StreetNameRegistry.Tests.AggregateTests.WhenCorrectingMunicipalityToCurrent
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

    public sealed class GivenMunicipalityWasAlreadyCurrent : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;
        private readonly MunicipalityStreamId _streamId;

        public GivenMunicipalityWasAlreadyCurrent(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
            _streamId = Fixture.Create<MunicipalityStreamId>();
        }

        [Fact]
        public void ThenNone()
        {
            var commandCorrectMunicipality = Fixture.Create<CorrectToCurrentMunicipality>();
            Assert(new Scenario()
                .Given(_streamId, new object[]
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
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityWasRetired>()
                })
                .When(commandCorrectMunicipality)
                .Then(new[]
                {
                    new Fact(_streamId, new MunicipalityWasCorrectedToCurrent(_municipalityId))
                }));
        }
    }
}
