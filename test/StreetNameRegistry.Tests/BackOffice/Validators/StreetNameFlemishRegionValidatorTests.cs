namespace StreetNameRegistry.Tests.BackOffice.Validators
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using StreetNameRegistry.Api.BackOffice.Validators;
    using Xunit;

    public sealed class StreetNameFlemishRegionValidatorTests
    {
        private readonly TestConsumerContext _testConsumerContext;
        private string puriPattern = "http://uri/{0}";

        public StreetNameFlemishRegionValidatorTests()
        {
            _testConsumerContext = new FakeConsumerContextFactory().CreateDbContext();
        }

        [Theory]
        [InlineData("11001", true)]
        [InlineData("55001", false)]
        public async Task GivenMunicipalityExists_ThenReturnsTrue(string nisCode, bool expectedResult)
        {
            _testConsumerContext.AddMunicipalityLatestItemFixtureWithNisCode(nisCode);

            var validator = new StreetNameFlemishRegionValidator(_testConsumerContext);

            var result = await validator.IsValidAsync(string.Format(puriPattern, nisCode), CancellationToken.None);

            result.Should().Be(expectedResult);
        }
    }
}
