namespace StreetNameRegistry.Tests.AggregateTests.WhenSettingMunicipalityToCurrent
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

    public class GivenMunicipality : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;
        private readonly MunicipalityStreamId _streamId;

        public GivenMunicipality(ITestOutputHelper output) : base(output)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
            _streamId = Fixture.Create<MunicipalityStreamId>();
        }

        [Fact]
        public void ThenMunicipalityBecameCurrent()
        {
            var command = Fixture.Create<SetMunicipalityToCurrent>()
                .WithMunicipalityId(_municipalityId);

            Assert(new Scenario()
                .Given(_streamId,
                    Fixture.Create<MunicipalityWasImported>())
                .When(command)
                .Then(new[]
                {
                    new Fact(_streamId, new MunicipalityBecameCurrent(command.MunicipalityId))
                }));
        }

        [Fact]
        public void WhenMunicipalityAlreadyBecameCurrentThenNothingHappens()
        {
            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var municipalityBecameCurrent = Fixture.Create<MunicipalityBecameCurrent>();
            var command = Fixture.Create<SetMunicipalityToCurrent>()
                .WithMunicipalityId(new MunicipalityId(_municipalityId));

            Assert(new Scenario()
                .Given(
                    _streamId,
                    municipalityWasImported,
                    municipalityBecameCurrent
                )
                .When(command)
                .ThenNone());
        }
    }

    public static class SetMunicipalityToCurrentExtensions
    {
        public static SetMunicipalityToCurrent WithMunicipalityId(
            this SetMunicipalityToCurrent command,
            MunicipalityId municipalityId)
        {
            return new SetMunicipalityToCurrent(municipalityId, command.Provenance);
        }
    }
}
