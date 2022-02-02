namespace StreetNameRegistry.Tests.AggregateTests.WhenRetiringMunicipality
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

    public class GivenMunicipality : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;

        public GivenMunicipality(ITestOutputHelper output) : base(output)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
        }

        [Fact]
        public void ThenMunicipalityWasRetired()
        {
            var command = Fixture.Create<RetireMunicipality>()
                .WithMunicipalityId(_municipalityId);

            Assert(new Scenario()
                .Given(_municipalityId,
                    Fixture.Create<MunicipalityWasImported>())
                .When(command)
                .Then(new Fact(_municipalityId, new MunicipalityWasRetired(command.MunicipalityId))));
        }

        [Fact]
        public void WhenMunicipalityAlreadyRetiredThenNothingHappens()
        {
            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var municipalityWasRetired = Fixture.Create<MunicipalityWasRetired>();
            var command = Fixture.Create<RetireMunicipality>()
                .WithMunicipalityId(new MunicipalityId(_municipalityId));

            Assert(new Scenario()
                .Given(
                    _municipalityId,
                    municipalityWasImported,
                    municipalityWasRetired
                )
                .When(command)
                .ThenNone());
        }
    }

    public static class RetireMunicipalityExtensions
    {
        public static RetireMunicipality WithMunicipalityId(
            this RetireMunicipality command,
            MunicipalityId municipalityId)
        {
            return new RetireMunicipality(municipalityId, command.Provenance);
        }
    }
}
