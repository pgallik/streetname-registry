namespace StreetNameRegistry.Tests.AggregateTests.WhenMigratingStreetName
{
    using System;
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
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
        private readonly MunicipalityStreamId _streamId;

        public GivenMunicipality(ITestOutputHelper output) : base(output)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
            _streamId = Fixture.Create<MunicipalityStreamId>();
        }

        [Fact]
        public void ThenStreetNameWasMigrated()
        {
            var command = Fixture.Create<MigrateStreetNameToMunicipality>();

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();

            // Act, Assert
            Assert(new Scenario()
                .Given(_streamId, municipalityWasImported)
                .When(command)
                .Then(
                    new Fact(_streamId, new StreetNameWasMigratedToMunicipality(
                        _municipalityId,
                        new NisCode(municipalityWasImported.NisCode),
                        command.StreetNameId,
                        command.PersistentLocalId,
                        command.Status,
                        command.PrimaryLanguage,
                        command.SecondaryLanguage,
                        command.Names,
                        command.HomonymAdditions,
                        command.IsCompleted,
                        command.IsRemoved))
                ));
        }

        [Fact]
        public void WithExistingStreetNamePersistentLocalId_ThenThrowsInvalidOperationException()
        {
            var persistentLocalId = 123456;
            Fixture.Register(() => new PersistentLocalId(persistentLocalId));

            var command = Fixture.Create<MigrateStreetNameToMunicipality>();

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var streetNameMigratedToMunicipality = Fixture.Create<StreetNameWasMigratedToMunicipality>();

            // Act, Assert
            Assert(new Scenario()
                .Given(_streamId, municipalityWasImported, streetNameMigratedToMunicipality)
                .When(command)
                .Throws(new InvalidOperationException($"Cannot migrate StreetName with id '{persistentLocalId}' in municipality '{_municipalityId}'.")));
        }
    }
}
