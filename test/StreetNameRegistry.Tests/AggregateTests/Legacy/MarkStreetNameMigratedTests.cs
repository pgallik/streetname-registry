namespace StreetNameRegistry.Tests.AggregateTests.Legacy
{
    using System;
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using StreetName.Commands;
    using StreetName.Events;
    using StreetName.Events.Crab;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class MarkStreetNameMigratedTests : StreetNameRegistryTest
    {
        private readonly StreetNameId _streetNameId;

        public MarkStreetNameMigratedTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedStreetNameId());
            _streetNameId = Fixture.Create<StreetNameId>();
        }

        [Fact]
        public void GivenStreetName_WhenMarkStreetNameMigrated_ThenStreetNameWasMigrated()
        {
            var command = Fixture.Create<MarkStreetNameMigrated>();
            var streetNamePersistentLocalIdWasAssigned = Fixture.Create<StreetNamePersistentLocalIdWasAssigned>();

            Assert(new Scenario()
                .Given(_streetNameId,
                    Fixture.Create<StreetNameWasRegistered>(),
                    Fixture.Create<StreetNameWasNamed>(),
                    Fixture.Create<StreetNamePrimaryLanguageWasDefined>(),
                    Fixture.Create<StreetNameHomonymAdditionWasDefined>(),
                    streetNamePersistentLocalIdWasAssigned,
                    Fixture.Create<StreetNameWasImportedFromCrab>(),

                    Fixture.Create<StreetNameWasProposed>(),
                    Fixture.Create<StreetNameStatusWasImportedFromCrab>()
                )
                .When(command)
                .Then(new Fact(_streetNameId, new StreetNameWasMigrated(_streetNameId, command.MunicipalityId, new PersistentLocalId(streetNamePersistentLocalIdWasAssigned.PersistentLocalId)))));
        }

        [Fact]
        public void GivenStreetNameMigrated_WhenMarkStreetNameMigrated_ThenNone()
        {
            Assert(new Scenario()
                .Given(_streetNameId,
                    Fixture.Create<StreetNameWasRegistered>(),
                    Fixture.Create<StreetNameWasNamed>(),
                    Fixture.Create<StreetNamePrimaryLanguageWasDefined>(),
                    Fixture.Create<StreetNameHomonymAdditionWasDefined>(),
                    Fixture.Create<StreetNamePersistentLocalIdWasAssigned>(),
                    Fixture.Create<StreetNameWasImportedFromCrab>(),

                    Fixture.Create<StreetNameWasProposed>(),
                    Fixture.Create<StreetNameStatusWasImportedFromCrab>(),
                    Fixture.Create<StreetNameWasMigrated>()
                )
                .When(Fixture.Create<MarkStreetNameMigrated>())
                .ThenNone());
        }

        [Fact]
        public void GivenStreetNameMigrated_WhenImportingFromCrab_ThenExceptionIsExpected()
        {
            Assert(new Scenario()
                .Given(_streetNameId,
                    Fixture.Create<StreetNameWasRegistered>(),
                    Fixture.Create<StreetNameWasNamed>(),
                    Fixture.Create<StreetNamePrimaryLanguageWasDefined>(),
                    Fixture.Create<StreetNameHomonymAdditionWasDefined>(),
                    Fixture.Create<StreetNamePersistentLocalIdWasAssigned>(),
                    Fixture.Create<StreetNameWasImportedFromCrab>(),

                    Fixture.Create<StreetNameWasProposed>(),
                    Fixture.Create<StreetNameStatusWasImportedFromCrab>(),
                    Fixture.Create<StreetNameWasMigrated>()
                )
                .When(Fixture.Create<ImportStreetNameFromCrab>())
                .Throws(new InvalidOperationException($"The StreetName aggregate {_streetNameId} has been migrated!")));
        }

        [Fact]
        public void GivenStreetNameMigrated_WhenImportingStatusFromCrab_ThenExceptionIsExpected()
        {
            Assert(new Scenario()
                .Given(_streetNameId,
                    Fixture.Create<StreetNameWasRegistered>(),
                    Fixture.Create<StreetNameWasNamed>(),
                    Fixture.Create<StreetNamePrimaryLanguageWasDefined>(),
                    Fixture.Create<StreetNameHomonymAdditionWasDefined>(),
                    Fixture.Create<StreetNamePersistentLocalIdWasAssigned>(),
                    Fixture.Create<StreetNameWasImportedFromCrab>(),

                    Fixture.Create<StreetNameWasProposed>(),
                    Fixture.Create<StreetNameStatusWasImportedFromCrab>(),
                    Fixture.Create<StreetNameWasMigrated>()
                )
                .When(Fixture.Create<ImportStreetNameStatusFromCrab>())
                .Throws(new InvalidOperationException($"The StreetName aggregate {_streetNameId} has been migrated!")));
        }
    }
}
