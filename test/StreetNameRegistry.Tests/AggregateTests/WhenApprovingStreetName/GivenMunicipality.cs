namespace StreetNameRegistry.Tests.AggregateTests.WhenApprovingStreetName
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using FluentAssertions;
    using Municipality;
    using Testing;
    using Xunit.Abstractions;
    using global::AutoFixture;
    using Municipality.Commands;
    using Xunit;
    using Municipality.Events;
    using Municipality.Exceptions;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;

    public class GivenMunicipality : StreetNameRegistryTest
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
        public void ThenStreetNameWasApproved()
        {
            var command = Fixture.Create<ApproveStreetName>()
                .WithMunicipalityId(_municipalityId);

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();
            var streetNameWasProposed = Fixture.Create<StreetNameWasProposedV2>();

            // Act, assert
            Assert(new Scenario()
                .Given(_streamId, municipalityWasImported, streetNameWasProposed)
                .When(command)
                .Then(new Fact(_streamId, new StreetNameWasApproved(_municipalityId, command.PersistentLocalId))));
        }

        [Fact]
        public void ThenStreetNameNotFoundExceptionWasThrown()
        {
            var command = Fixture.Create<ApproveStreetName>()
                .WithMunicipalityId(_municipalityId);

            var municipalityWasImported = Fixture.Create<MunicipalityWasImported>();

            // Act, assert
            Assert(new Scenario()
                .Given(_streamId, municipalityWasImported)
                .When(command)
                .Throws(new StreetNameNotFoundException(command.PersistentLocalId)));
        }

        [Fact]
        public void ThenStreetNameWasRemovedExceptionWasThrown()
        {
            var command = Fixture.Create<ApproveStreetName>()
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
                        StreetNameStatus.Current,
                        Language.Dutch,
                        null,
                        Fixture.Create<Names>(),
                        new HomonymAdditions(),
                        true,
                        true);

                    ((ISetProvenance)streetNameWasMigratedToMunicipality).SetProvenance(Fixture.Create<Provenance>());
                    return streetNameWasMigratedToMunicipality;
                })
                .Create();


            // Act, assert
            Assert(new Scenario()
                .Given(_streamId, municipalityWasImported, streetNameMigratedToMunicipality)
                .When(command)
                .Throws(new StreetNameWasRemovedException(command.PersistentLocalId)));
        }

        [Fact]
        public void ThenStreetNameStatusPreventsApprovalExceptionWasThrown()
        {
            var command = Fixture.Create<ApproveStreetName>()
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
                        StreetNameStatus.Current,
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
                .Given(_streamId, municipalityWasImported, streetNameMigratedToMunicipality)
                .When(command)
                .Throws(new StreetNameStatusPreventsApprovalException(command.PersistentLocalId)));
        }

        [Fact]
        public void ThenStreetNameStatusIsCurrent()
        {
            var persistentLocalId = Fixture.Create<PersistentLocalId>();
            var aggregate = Municipality.Factory();
            aggregate.Initialize(new List<object>
            {
                Fixture.Create<MunicipalityWasImported>(),
                Fixture.Create<StreetNameWasProposedV2>()
            });

            // Act
            aggregate.ApproveStreetName(persistentLocalId);

            // Assert
            var result = aggregate.StreetNames.GetByPersistentLocalId(persistentLocalId);
            result.Status.Should().Be(StreetNameStatus.Current);
        }
    }
}
