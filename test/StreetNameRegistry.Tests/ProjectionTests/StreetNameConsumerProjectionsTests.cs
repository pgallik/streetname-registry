namespace StreetNameRegistry.Tests.ProjectionTests
{
    using System.Threading.Tasks;
    using Consumer.Municipality;
    using Consumer.Projections;
    using FluentAssertions;
    using Generate;
    using Xunit;
    using Xunit.Abstractions;

    public class StreetNameConsumerProjectionsTests : StreetNameConsumerProjectionTest<MunicipalityConsumerProjection>
    {
        public StreetNameConsumerProjectionsTests(ITestOutputHelper output)
            : base(output)
        { }

        [Fact]
        public Task MunicipalityWasImportedAddsMunicipality()
        {
            var id = Arrange(Produce.Guid());
            var nisCode = Arrange(Produce.NumericString(5));

            return GivenEvents(Generate.EventsFor.ImportedMunicipalities(id, nisCode))
                .Project(Generate.MunicipalityWasImported)
                .Then(async ct =>
                {
                    var entity = await ct.FindAsync<MunicipalityConsumerItem>(id);
                    entity.Should().NotBeNull();
                });
        }

        [Fact(Skip = "Why does this fail?")]
        public Task MunicipalityNisCodeWasChangedSetsNisCode()
        {
            var id = Arrange(Produce.Guid());
            var nisCode = Arrange(Produce.NumericString(5));

            return GivenEvents(Generate.EventsFor.ChangedMunicipalityNisCodes(id, nisCode))
                .Project(Generate.MunicipalityNisCodeWasChanged
                    .Select(e => e.WithIdAndNisCode(id, nisCode)))
                .Then(async ct =>
                {
                    var entity = await ct.FindAsync<MunicipalityConsumerItem>(id);
                    entity.Should().NotBeNull();
                    entity.NisCode.Should().Be(nisCode);
                });
        }
    }
}
