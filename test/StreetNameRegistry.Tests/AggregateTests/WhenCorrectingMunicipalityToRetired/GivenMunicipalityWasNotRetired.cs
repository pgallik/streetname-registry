namespace StreetNameRegistry.Tests.AggregateTests.WhenCorrectingMunicipalityToRetired
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

    public class GivenMunicipalityWasNotRetired : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;

        public GivenMunicipalityWasNotRetired(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
        }

        [Fact]
        public void ThenMunicipalityGetsCorrectedToRetired()
        {
            var commandCorrectMunicipality = Fixture.Create<CorrectToRetiredMunicipality>();
            Assert(new Scenario()
                .Given(_municipalityId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                })
                .When(commandCorrectMunicipality)
                .Then(new[]
                {
                    new Fact(_municipalityId, new MunicipalityWasCorrectedToRetired(_municipalityId))
                }));
        }

        [Fact]
        public void AndCorrectedToRetired_ThenNone()
        {
            var commandCorrectMunicipality = Fixture.Create<CorrectToRetiredMunicipality>();
            Assert(new Scenario()
                .Given(_municipalityId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityWasCorrectedToRetired>(),
                })
                .When(commandCorrectMunicipality)
                .ThenNone());
        }
    }
}
