namespace StreetNameRegistry.Tests.ProjectionTests
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Consumer.Municipality;
    using Consumer.Projections;
    using FluentAssertions;
    using global::AutoFixture;
    using StreetName.Events;
    using Xunit;

    public class StreetNameConsumerProjectionsTests : StreetNameConsumerProjectionTest<MunicipalityConsumerProjection>
    {
        private readonly Fixture _fixture;

        public StreetNameConsumerProjectionsTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new InfrastructureCustomization());
            _fixture.Customize(new WithFixedMunicipalityId());
        }

        [Fact]
        public async Task MunicipalityWasImportedAddsMunicipality()
        {
            var municipalityWasImported = _fixture.Create<MunicipalityWasImported>();

            await Sut
                .Given(municipalityWasImported)
                .Then(async ct =>
                {
                    var expectedMunicipality = await ct.FindAsync<MunicipalityConsumerItem>(municipalityWasImported.MunicipalityId);
                    expectedMunicipality.Should().NotBeNull();
                    expectedMunicipality.MunicipalityId.Should().Be(municipalityWasImported.MunicipalityId);
                    expectedMunicipality.NisCode.Should().Be(municipalityWasImported.NisCode);
                });
        }

        [Fact]
        public async Task MunicipalityNisCodeWasChangedSetsNisCode()
        {
            var municipalityWasImported = _fixture.Create<MunicipalityWasImported>();
            var municipalityNisCodeWasChanged = _fixture.Create<MunicipalityNisCodeWasChanged>();

            await Sut
                .Given(municipalityWasImported, municipalityNisCodeWasChanged)
                .Then(async ct =>
                {
                    var expectedMunicipality = await ct.FindAsync<MunicipalityConsumerItem>(municipalityWasImported.MunicipalityId);
                    expectedMunicipality.Should().NotBeNull();
                    expectedMunicipality.MunicipalityId.Should().Be(municipalityWasImported.MunicipalityId);
                    expectedMunicipality.NisCode.Should().Be(municipalityNisCodeWasChanged.NisCode);
                });
        }
    }
}
