namespace StreetNameRegistry.Tests.AggregateTests.WhenRetiringStreetName
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Snapshotting;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands;
    using Municipality.Events;
    using Municipality.Exceptions;
    using Extensions;
    using System.Collections.Generic;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class GivenMunicipality : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;
        private readonly MunicipalityStreamId _streamId;

        public GivenMunicipality(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            Fixture.Customize(new WithFixedPersistentLocalId());
            _municipalityId = Fixture.Create<MunicipalityId>();
            _streamId = Fixture.Create<MunicipalityStreamId>();
        }

        [Fact]
        public void ThenStreetNameWasRetired()
        {
            var command = Fixture.Create<RetireStreetName>()
                .WithMunicipalityId(_municipalityId);

            // Act, assert
            Assert(new Scenario()
                .Given(_streamId,
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityBecameCurrent>(),
                    Fixture.Create<StreetNameWasProposedV2>(),
                    Fixture.Create<StreetNameWasApproved>())
                .When(command)
                .Then(new Fact(_streamId, new StreetNameWasRetiredV2(_municipalityId, command.PersistentLocalId))));
        }

        [Fact]
        public void ThenStreetNameNotFoundExceptionWasThrown()
        {
            var command = Fixture.Create<RetireStreetName>()
                .WithMunicipalityId(_municipalityId);

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();

            // Act, assert
            Assert(new Scenario()
                .Given(_streamId, municipalityWasImported)
                .When(command)
                .Throws(new StreetNameIsNotFoundException(command.PersistentLocalId)));
        }

        [Fact]
        public void ThenStreetNameIsRemovedExceptionWasThrown()
        {
            var command = Fixture.Create<RetireStreetName>()
                .WithMunicipalityId(_municipalityId);

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var municipalityBecameCurrent = Fixture.Create<MunicipalityBecameCurrent>();
            var removedStreetNameMigratedToMunicipality = Fixture.Build<StreetNameWasMigratedToMunicipality>()
                .FromFactory(() =>
                {
                    var streetNameWasMigratedToMunicipality = new StreetNameWasMigratedToMunicipality(
                        _municipalityId,
                        Fixture.Create<NisCode>(),
                        Fixture.Create<StreetNameId>(),
                        Fixture.Create<PersistentLocalId>(),
                        StreetNameStatus.Current,
                        Language.Dutch,
                        null,
                        Fixture.Create<Names>(),
                        new HomonymAdditions(),
                        true,
                        isRemoved: true);

                    ((ISetProvenance)streetNameWasMigratedToMunicipality).SetProvenance(Fixture.Create<Provenance>());
                    return streetNameWasMigratedToMunicipality;
                })
                .Create();


            // Act, assert
            Assert(new Scenario()
                .Given(_streamId,
                    municipalityWasImported,
                    municipalityBecameCurrent,
                    removedStreetNameMigratedToMunicipality)
                .When(command)
                .Throws(new StreetNameIsRemovedException(command.PersistentLocalId)));
        }

        [Fact]
        public void WithMunicipalityStatusRetired_ThenMunicipalityHasInvalidStatusExceptionWasThrown()
        {
            var command = Fixture.Create<RetireStreetName>()
                .WithMunicipalityId(_municipalityId);

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var streetNameMigratedToMunicipality = Fixture.Build<StreetNameWasMigratedToMunicipality>()
                .FromFactory(() =>
                {
                    var streetNameWasMigratedToMunicipality = new StreetNameWasMigratedToMunicipality(
                        _municipalityId,
                        Fixture.Create<NisCode>(),
                        Fixture.Create<StreetNameId>(),
                        Fixture.Create<PersistentLocalId>(),
                        StreetNameStatus.Proposed,
                        Language.Dutch,
                        null,
                        Fixture.Create<Names>(),
                        new HomonymAdditions(),
                        true,
                        isRemoved: false);

                    ((ISetProvenance)streetNameWasMigratedToMunicipality).SetProvenance(Fixture.Create<Provenance>());
                    return streetNameWasMigratedToMunicipality;
                })
                .Create();


            // Act, assert
            Assert(new Scenario()
                .Given(_streamId,
                    municipalityWasImported,
                    Fixture.Create<MunicipalityWasRetired>(),
                    streetNameMigratedToMunicipality)
                .When(command)
                .Throws(new MunicipalityHasInvalidStatusException()));
        }

        [Theory]
        [InlineData(StreetNameStatus.Proposed)]
        [InlineData(StreetNameStatus.Rejected)]
        public void ThenStreetNameHasInvalidStatusExceptionWasThrown(StreetNameStatus status)
        {
            var command = Fixture.Create<RetireStreetName>()
                .WithMunicipalityId(_municipalityId);

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var streetNameMigratedToMunicipality = Fixture.Build<StreetNameWasMigratedToMunicipality>()
                .FromFactory(() =>
                {
                    var streetNameWasMigratedToMunicipality = new StreetNameWasMigratedToMunicipality(
                        _municipalityId,
                        Fixture.Create<NisCode>(),
                        Fixture.Create<StreetNameId>(),
                        Fixture.Create<PersistentLocalId>(),
                        status,
                        Language.Dutch,
                        null,
                        Fixture.Create<Names>(),
                        new HomonymAdditions(),
                        true,
                        isRemoved: false);

                    ((ISetProvenance)streetNameWasMigratedToMunicipality).SetProvenance(Fixture.Create<Provenance>());
                    return streetNameWasMigratedToMunicipality;
                })
                .Create();


            // Act, assert
            Assert(new Scenario()
                .Given(_streamId,
                    municipalityWasImported,
                    Fixture.Create<MunicipalityBecameCurrent>(),
                    streetNameMigratedToMunicipality)
                .When(command)
                .Throws(new StreetNameHasInvalidStatusException(command.PersistentLocalId)));
        }

        [Fact]
        public void WithStreetNameAlreadyRetired_ThenNone()
        {
            var command = Fixture.Create<RetireStreetName>()
                .WithMunicipalityId(_municipalityId);

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var streetNameMigratedToMunicipality = Fixture.Build<StreetNameWasMigratedToMunicipality>()
                .FromFactory(() =>
                {
                    var streetNameWasMigratedToMunicipality = new StreetNameWasMigratedToMunicipality(
                        _municipalityId,
                        Fixture.Create<NisCode>(),
                        Fixture.Create<StreetNameId>(),
                        Fixture.Create<PersistentLocalId>(),
                        StreetNameStatus.Retired,
                        Language.Dutch,
                        null,
                        Fixture.Create<Names>(),
                        new HomonymAdditions(),
                        true,
                        isRemoved: false);

                    ((ISetProvenance)streetNameWasMigratedToMunicipality).SetProvenance(Fixture.Create<Provenance>());
                    return streetNameWasMigratedToMunicipality;
                })
                .Create();

            // Act, assert
            Assert(new Scenario()
                .Given(_streamId,
                    municipalityWasImported,
                    Fixture.Create<MunicipalityBecameCurrent>(),
                    streetNameMigratedToMunicipality)
                .When(command)
                .ThenNone());
        }

        [Fact]
        public void ThenStreetNameStatusIsRetired()
        {
            var persistentLocalId = Fixture.Create<PersistentLocalId>();
            var aggregate = new MunicipalityFactory(NoSnapshotStrategy.Instance).Create();
            aggregate.Initialize(new List<object>
            {
                Fixture.Create<MunicipalityWasImported>(),
                Fixture.Create<MunicipalityBecameCurrent>(),
                Fixture.Create<StreetNameWasProposedV2>(),
                Fixture.Create<StreetNameWasApproved>()
            });

            // Act
            aggregate.RetireStreetName(persistentLocalId);

            // Assert
            var result = aggregate.StreetNames.GetByPersistentLocalId(persistentLocalId);
            result.Status.Should().Be(StreetNameStatus.Retired);
        }
    }
}
