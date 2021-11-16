namespace StreetNameRegistry.Tests.ProjectionTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Projections.Legacy;
    using Testing;
    using Xunit.Abstractions;
    using global::AutoFixture;
    using Projections.Legacy.StreetNameNameV2;
    using StreetName;
    using StreetName.Events;
    using Xunit;

    public class StreetNameNameProjectionsV2Tests : ProjectionTest<LegacyContext, StreetNameNameProjectionsV2>
    {
        private readonly Fixture? _fixture;

        public StreetNameNameProjectionsV2Tests(ITestOutputHelper output) : base(output)
        {
            _fixture = new Fixture();
            _fixture.Customize(new InfrastructureCustomization());
            _fixture.Customize(new WithFixedMunicipalityId());
        }

        [Fact]
        public async Task WhenStreetNameWasProposed_ThenStreetNameWasAdded()
        {
            _fixture.Register(() => new Names(_fixture.CreateMany<StreetNameName>(2).ToList()));
            var streetNameWasProposedV2 = _fixture.Create<StreetNameWasProposedV2>();

            await Given(streetNameWasProposedV2)
                .Then(async ct =>
                {
                    var expectedStreetName = (await ct.FindAsync<StreetNameNameV2>(streetNameWasProposedV2.PersistentLocalId));
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
                });
        }

        [Fact]
        public async Task WhenMunicipalityNisCodeWasChanged_ThenMunicipalityNisCodeIsUpdatedAndLinkedStreetNamesHaveNewNisCode()
        {
            _fixture.Register(() => new Names(_fixture.CreateMany<StreetNameName>(2).ToList()));
            var streetNameWasProposedV2 = _fixture.Create<StreetNameWasProposedV2>();
            var streetNameWasProposedV2_2 = _fixture.Create<StreetNameWasProposedV2>();
            var municipalityNisCodeWasChanged = _fixture.Create<MunicipalityNisCodeWasChanged>();

            await Given(
                    streetNameWasProposedV2,
                    streetNameWasProposedV2_2,
                    municipalityNisCodeWasChanged)
                .Then(async ct =>
                {
                   var expectedStreetNames = ct.StreetNameNamesV2.Where(x =>
                        x.MunicipalityId == municipalityNisCodeWasChanged.MunicipalityId);

                    expectedStreetNames.Select(x => x.NisCode)
                        .Distinct()
                        .Should()
                        .BeEquivalentTo(new List<string> { municipalityNisCodeWasChanged.NisCode });
                });
        }

        private string DetermineExpectedNameForLanguage(IEnumerable<StreetNameName> streetNameNames, Language language)
        {
            return streetNameNames.SingleOrDefault(x => x.Language == language)?.Name;
        }

        protected override LegacyContext CreateContext(DbContextOptions<LegacyContext> options)
        {
            return new LegacyContext(options);
        }
    }
}
