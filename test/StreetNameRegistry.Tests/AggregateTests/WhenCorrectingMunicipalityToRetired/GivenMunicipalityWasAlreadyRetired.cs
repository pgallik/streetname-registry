namespace StreetNameRegistry.Tests.AggregateTests.WhenCorrectingMunicipalityToRetired
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

    public sealed class GivenMunicipalityWasAlreadyRetired : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;
        private readonly MunicipalityStreamId _streamId;

        public GivenMunicipalityWasAlreadyRetired(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
            _streamId = Fixture.Create<MunicipalityStreamId>();
        }

        [Fact]
        public void ThenNone()
        {
            var commandCorrectMunicipality = Fixture.Create<CorrectToRetiredMunicipality>();
            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityWasRetired>()
                })
                .When(commandCorrectMunicipality)
                .ThenNone());
        }

        [Fact]
        public void AndChangedToCurrent_ThenItIsChangedToRetired()
        {
            var commandCorrectMunicipality = Fixture.Create<CorrectToRetiredMunicipality>();
            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityBecameCurrent>()
                })
                .When(commandCorrectMunicipality)
                .Then(new[]
                {
                    new Fact(_streamId, new MunicipalityWasCorrectedToRetired(_municipalityId))
                }));
        }
    }
}
