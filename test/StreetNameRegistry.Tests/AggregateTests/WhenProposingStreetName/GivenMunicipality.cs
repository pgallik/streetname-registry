namespace StreetNameRegistry.Tests.AggregateTests.WhenProposingStreetName
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Exceptions;
    using global::AutoFixture;
    using StreetName;
    using StreetName.Commands;
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
        public void ThenStreetNameWasProposed()
        {
            var command = Fixture.Create<ProposeStreetName>()
                .WithMunicipalityId(_municipalityId)
                .WithRandomStreetName(Fixture);

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            Assert(new Scenario()
                .Given(_municipalityId, municipalityWasImported)
                .When(command)
                .Then(new[]
                {
                    new Fact(_municipalityId, new StreetNameWasProposedV2(_municipalityId, new NisCode(municipalityWasImported.NisCode), command.StreetNameNames, command.PersistentLocalId))
                }));

        }

        [Fact]
        public void WithExistingStreetName_ThenStreetNameNameAlreadyExistsExceptionWasThrown()
        {
            var streetNameName = Fixture.Create<StreetNameName>();
            Fixture.Register(() => new Names { streetNameName });

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var streetNameWasProposed = Fixture.Create<StreetNameWasProposedV2>();

            var command = Fixture.Create<ProposeStreetName>()
                .WithMunicipalityId(_municipalityId);

            Assert(new Scenario()
                .Given(_municipalityId,
                    municipalityWasImported,
                    streetNameWasProposed)
                .When(command)
                .Throws(new StreetNameNameAlreadyExistsException(streetNameName.Name)));
        }

        [Fact]
        public void WithOneExistingStreetNameAndOneNew_ThenStreetNameNameAlreadyExistsExceptionWasThrown()
        {
            var existingStreetNameName = Fixture.Create<StreetNameName>();
            var newStreetNameName = Fixture.Create<StreetNameName>();
            Fixture.Register(() => new Names { existingStreetNameName });

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var streetNameWasProposed = Fixture.Create<StreetNameWasProposedV2>();

            var command = Fixture.Create<ProposeStreetName>()
                .WithMunicipalityId(_municipalityId)
                .WithStreetNameNames(new Names { existingStreetNameName, newStreetNameName });

            Assert(new Scenario()
                .Given(_municipalityId,
                    municipalityWasImported,
                    streetNameWasProposed)
                .When(command)
                .Throws(new StreetNameNameAlreadyExistsException(existingStreetNameName.Name)));
        }

        [Fact]
        public void WithNoConflictingStreetNames_ThenStreetNameWasProposed()
        {
            var existingStreetNameName = Fixture.Create<StreetNameName>();
            var newStreetNameName = Fixture.Create<StreetNameName>();
            Fixture.Register(() => new Names { existingStreetNameName });

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var streetNameWasProposed = Fixture.Create<StreetNameWasProposedV2>();

            var command = Fixture.Create<ProposeStreetName>()
                .WithMunicipalityId(_municipalityId)
                .WithStreetNameNames(new Names { newStreetNameName });

            Assert(new Scenario()
                .Given(_municipalityId,
                    municipalityWasImported,
                    streetNameWasProposed)
                .When(command)
                .Then(new[]
                {
                    new Fact(_municipalityId, new StreetNameWasProposedV2(_municipalityId, new NisCode(municipalityWasImported.NisCode), command.StreetNameNames, command.PersistentLocalId))
                }));
        }

        [Fact]
        public void WithMunicipalityRetired_ThenMunicipalityWasRetiredExceptionWasThrown()
        {
            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var municipalityWasRetired = Fixture.Create<MunicipalityWasRetired>();

            var command = Fixture.Create<ProposeStreetName>()
                .WithMunicipalityId(_municipalityId)
                .WithRandomStreetName(Fixture);

            Assert(new Scenario()
                .Given(_municipalityId,
                    municipalityWasImported,
                    municipalityWasRetired)
                .When(command)
                .Throws(new MunicipalityWasRetiredException($"Municipality with id '{_municipalityId}' was retired")));
        }
    }
}
