namespace StreetNameRegistry.Tests.AggregateTests.WhenChangingMunicipalityNisCode
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands;
    using Municipality.Events;
    using Municipality.Exceptions;
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
        public void ThenNisCodeChanged()
        {
            var command = Fixture.Create<ChangeMunicipalityNisCode>()
                .WithMunicipalityId(_municipalityId);

            Assert(new Scenario()
                .Given(_streamId,
                    Fixture.Create<MunicipalityWasImported>())
                .When(command)
                .Then(new[]
                {
                    new Fact(_streamId,
                        new MunicipalityNisCodeWasChanged(command.MunicipalityId, command.NisCode))
                }));
        }

        [Fact]
        public void ThenNisCodeChangedToNisCodeRonse()
        {
            var nisCode = new NisCode("45041");
            var command = Fixture.Create<ChangeMunicipalityNisCode>()
                .WithMunicipalityId(_municipalityId)
                .WithNisCode(nisCode);

            Assert(new Scenario()
                .Given(_streamId,
                    Fixture.Create<MunicipalityWasImported>())
                .When(command)
                .Then(new[]
                {
                    new Fact(_streamId, new MunicipalityNisCodeWasChanged(command.MunicipalityId, nisCode))
                }));
        }

        [Fact]
        public void ThenNisCodeChangedToNull()
        {
            var command = Fixture.Create<ChangeMunicipalityNisCode>()
                .WithMunicipalityId(_municipalityId)
                .WithNisCode(null);

            Assert(new Scenario()
                .Given(_streamId,
                    Fixture.Create<MunicipalityWasImported>())
                .When(command)
                .Throws(new NoNisCodeHasNoValueException("NisCode of a municipality cannot be empty.")));
        }

        [Fact]
        public void WithSameNisCodeThenNothingHappens()
        {
            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var command = Fixture.Create<ChangeMunicipalityNisCode>()
                .WithMunicipalityId(_municipalityId)
                .WithNisCode(new NisCode(municipalityWasImported.NisCode));

            Assert(new Scenario()
                .Given(_streamId, municipalityWasImported)
                .When(command)
                .ThenNone());
        }
    }

    public static class MunicipalityNisCodeWasChangedExtensions
    {
        public static ChangeMunicipalityNisCode WithMunicipalityId(
            this ChangeMunicipalityNisCode command,
            MunicipalityId municipalityId)
        {
            return new ChangeMunicipalityNisCode(municipalityId, new NisCode(command.NisCode), command.Provenance);
        }

        public static ChangeMunicipalityNisCode WithNisCode(
            this ChangeMunicipalityNisCode command,
            NisCode nisCode)
        {
            return new ChangeMunicipalityNisCode(new MunicipalityId(command.MunicipalityId), nisCode,
                command.Provenance);
        }
    }
}
