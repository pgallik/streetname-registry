namespace StreetNameRegistry.Tests.ProjectionTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Pipes;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using FluentAssertions;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Events;
    using Projections.Legacy.StreetNameDetailV2;
    using Xunit;

    public class StreetNameLatestItemProjectionsV2Tests : StreetNameLegacyProjectionTest<StreetNameDetailProjectionsV2>
    {
        private readonly Fixture? _fixture;

        public StreetNameLatestItemProjectionsV2Tests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new InfrastructureCustomization());
        }

        [Fact]
        public async Task WhenStreetNameWasProposedV2_ThenNewStreetNameWasAdded()
        {
            _fixture.Register(() => new Names(_fixture.CreateMany<StreetNameName>(2).ToList()));
            var streetNameWasProposedV2 = _fixture.Create<StreetNameWasProposedV2>();

            var metadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, streetNameWasProposedV2.GetHash() }
            };

            await Sut
                .Given(new Envelope<StreetNameWasProposedV2>(new Envelope(streetNameWasProposedV2, metadata)))
                .Then(async ct =>
                {
                    var expectedStreetName = (await ct.FindAsync<StreetNameDetailV2>(streetNameWasProposedV2.PersistentLocalId));
                    expectedStreetName.Should().NotBeNull();
                    expectedStreetName.MunicipalityId.Should().Be(streetNameWasProposedV2.MunicipalityId);
                    expectedStreetName.NisCode.Should().Be(streetNameWasProposedV2.NisCode);
                    expectedStreetName.PersistentLocalId.Should().Be(streetNameWasProposedV2.PersistentLocalId);
                    expectedStreetName.Removed.Should().BeFalse();
                    expectedStreetName.Status.Should().Be(StreetNameStatus.Proposed);
                    expectedStreetName.VersionTimestamp.Should().Be(streetNameWasProposedV2.Provenance.Timestamp);
                    expectedStreetName.NameDutch.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.Dutch));
                    expectedStreetName.NameFrench.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.French));
                    expectedStreetName.NameGerman.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.German));
                    expectedStreetName.NameEnglish.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.English));
                    expectedStreetName.LastEventHash.Should().Be(streetNameWasProposedV2.GetHash());
                });
        }

        [Fact]
        public async Task WhenStreetNameWasApproved_ThenStreetNameStatusWasChangedToApproved()
        {
            _fixture.Register(() => new Names(_fixture.CreateMany<StreetNameName>(2).ToList()));
            var streetNameWasProposedV2 = _fixture.Create<StreetNameWasProposedV2>();
            var streetNameWasProposedV2Metadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, streetNameWasProposedV2.GetHash() }
            };

            var streetNameWasApproved = new StreetNameWasApproved(
                _fixture.Create<MunicipalityId>(),
                new PersistentLocalId(streetNameWasProposedV2.PersistentLocalId));
            ((ISetProvenance)streetNameWasApproved).SetProvenance(_fixture.Create<Provenance>());
            var streetNameWasApprovedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, streetNameWasApproved.GetHash() }
            };

            await Sut
                .Given(
                    new Envelope<StreetNameWasProposedV2>(new Envelope(streetNameWasProposedV2, streetNameWasProposedV2Metadata)),
                    new Envelope<StreetNameWasApproved>(new Envelope(streetNameWasApproved, streetNameWasApprovedMetadata)))
                .Then(async ct =>
                {
                    var expectedStreetName = (await ct.FindAsync<StreetNameDetailV2>(streetNameWasProposedV2.PersistentLocalId));
                    expectedStreetName.Should().NotBeNull();
                    expectedStreetName.MunicipalityId.Should().Be(streetNameWasProposedV2.MunicipalityId);
                    expectedStreetName.NisCode.Should().Be(streetNameWasProposedV2.NisCode);
                    expectedStreetName.PersistentLocalId.Should().Be(streetNameWasProposedV2.PersistentLocalId);
                    expectedStreetName.Removed.Should().BeFalse();
                    expectedStreetName.Status.Should().Be(StreetNameStatus.Current);
                    expectedStreetName.VersionTimestamp.Should().Be(streetNameWasApproved.Provenance.Timestamp);
                    expectedStreetName.NameDutch.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.Dutch));
                    expectedStreetName.NameFrench.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.French));
                    expectedStreetName.NameGerman.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.German));
                    expectedStreetName.NameEnglish.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.English));
                    expectedStreetName.LastEventHash.Should().Be(streetNameWasApproved.GetHash());
                });
        }

        [Fact]
        public async Task WhenStreetNameWasRejected_ThenStreetNameStatusWasChangedToRejected()
        {
            _fixture.Register(() => new Names(_fixture.CreateMany<StreetNameName>(2).ToList()));
            var streetNameWasProposedV2 = _fixture.Create<StreetNameWasProposedV2>();
            var streetNameWasProposedV2Metadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, streetNameWasProposedV2.GetHash() }
            };

            var streetNameWasRejected = new StreetNameWasRejected(
                _fixture.Create<MunicipalityId>(),
                new PersistentLocalId(streetNameWasProposedV2.PersistentLocalId));
            ((ISetProvenance)streetNameWasRejected).SetProvenance(_fixture.Create<Provenance>());
            var streetNameWasRejectedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, streetNameWasRejected.GetHash() }
            };

            await Sut
                .Given(
                    new Envelope<StreetNameWasProposedV2>(new Envelope(streetNameWasProposedV2, streetNameWasProposedV2Metadata)),
                    new Envelope<StreetNameWasRejected>(new Envelope(streetNameWasRejected, streetNameWasRejectedMetadata)))
                .Then(async ct =>
                {
                    var expectedStreetName = (await ct.FindAsync<StreetNameDetailV2>(streetNameWasProposedV2.PersistentLocalId));
                    expectedStreetName.Should().NotBeNull();
                    expectedStreetName.MunicipalityId.Should().Be(streetNameWasProposedV2.MunicipalityId);
                    expectedStreetName.NisCode.Should().Be(streetNameWasProposedV2.NisCode);
                    expectedStreetName.PersistentLocalId.Should().Be(streetNameWasProposedV2.PersistentLocalId);
                    expectedStreetName.Removed.Should().BeFalse();
                    expectedStreetName.Status.Should().Be(StreetNameStatus.Rejected);
                    expectedStreetName.VersionTimestamp.Should().Be(streetNameWasRejected.Provenance.Timestamp);
                    expectedStreetName.NameDutch.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.Dutch));
                    expectedStreetName.NameFrench.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.French));
                    expectedStreetName.NameGerman.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.German));
                    expectedStreetName.NameEnglish.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.English));
                    expectedStreetName.LastEventHash.Should().Be(streetNameWasRejected.GetHash());
                });
        }

        [Fact]
        public async Task WhenStreetNameWasRetiredV2_ThenStreetNameStatusWasChangedToRetired()
        {
            _fixture.Register(() => new Names(_fixture.CreateMany<StreetNameName>(2).ToList()));
            var streetNameWasProposedV2 = _fixture.Create<StreetNameWasProposedV2>();
            var streetNameWasProposedV2Metadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, streetNameWasProposedV2.GetHash() }
            };

            var streetNameWasApproved = new StreetNameWasApproved(
                _fixture.Create<MunicipalityId>(),
                new PersistentLocalId(streetNameWasProposedV2.PersistentLocalId));
            ((ISetProvenance)streetNameWasApproved).SetProvenance(_fixture.Create<Provenance>());
            var streetNameWasApprovedMetadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, streetNameWasApproved.GetHash() }
            };

            var streetNameWasRetiredV2 = new StreetNameWasRetiredV2(
                _fixture.Create<MunicipalityId>(),
                new PersistentLocalId(streetNameWasProposedV2.PersistentLocalId));
            ((ISetProvenance)streetNameWasRetiredV2).SetProvenance(_fixture.Create<Provenance>());
            var streetNameWasRetiredV2Metadata = new Dictionary<string, object>
            {
                { AddEventHashPipe.HashMetadataKey, streetNameWasRetiredV2.GetHash() }
            };

            await Sut
                .Given(
                    new Envelope<StreetNameWasProposedV2>(new Envelope(streetNameWasProposedV2, streetNameWasProposedV2Metadata)),
                    new Envelope<StreetNameWasApproved>(new Envelope(streetNameWasApproved, streetNameWasApprovedMetadata)),
                    new Envelope<StreetNameWasRetiredV2>(new Envelope(streetNameWasRetiredV2, streetNameWasRetiredV2Metadata)))
                .Then(async ct =>
                {
                    var expectedStreetName = (await ct.FindAsync<StreetNameDetailV2>(streetNameWasProposedV2.PersistentLocalId));
                    expectedStreetName.Should().NotBeNull();
                    expectedStreetName.MunicipalityId.Should().Be(streetNameWasProposedV2.MunicipalityId);
                    expectedStreetName.NisCode.Should().Be(streetNameWasProposedV2.NisCode);
                    expectedStreetName.PersistentLocalId.Should().Be(streetNameWasProposedV2.PersistentLocalId);
                    expectedStreetName.Removed.Should().BeFalse();
                    expectedStreetName.Status.Should().Be(StreetNameStatus.Retired);
                    expectedStreetName.VersionTimestamp.Should().Be(streetNameWasRetiredV2.Provenance.Timestamp);
                    expectedStreetName.NameDutch.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.Dutch));
                    expectedStreetName.NameFrench.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.French));
                    expectedStreetName.NameGerman.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.German));
                    expectedStreetName.NameEnglish.Should().Be(DetermineExpectedNameForLanguage(streetNameWasProposedV2.StreetNameNames, Language.English));
                    expectedStreetName.LastEventHash.Should().Be(streetNameWasRetiredV2.GetHash());
                });
        }

        private string DetermineExpectedNameForLanguage(IEnumerable<StreetNameName> streetNameNames, Language language)
        {
            return streetNameNames.SingleOrDefault(x => x.Language == language)?.Name;
        }
    }
}
