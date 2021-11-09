namespace StreetNameRegistry.Tests.BackOffice.Api.WhenProposingStreetName
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using global::AutoFixture;
    using Infrastructure;
using Moq;
    using StreetName.Commands;
    using StreetNameRegistry.Api.BackOffice.Infrastructure;
    using StreetNameRegistry.Api.BackOffice.StreetName;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using StreetNameRegistry.StreetName;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityExists : StreetNameRegistryBackOfficeTest
    {
        private readonly Fixture _fixture;
        private readonly StreetNameController _controller;
        private readonly TestSyndicationContext _syndicationContext;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityExists(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _controller = CreateApiBusControllerWithUser<StreetNameController>("John Doe");
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _syndicationContext = new FakeSyndicationContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public async Task ThenTheStreetNameIsProposed()
        {
            const int expectedLocation = 5;

            //Arrange
            var municipalityLatestItem = _syndicationContext.AddMunicipalityLatestItemFixture();
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(expectedLocation));

            var importMunicipality = new ImportMunicipality(
                new MunicipalityId(municipalityLatestItem.MunicipalityId),
                new NisCode(municipalityLatestItem.NisCode),
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(importMunicipality);

            var body = new StreetNameProposeRequest()
            {
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{municipalityLatestItem.NisCode}",
                Straatnamen = new Dictionary<Taal, string>()
                {
                    {Taal.NL, "Rodekruisstraat"},
                    {Taal.FR, "Rue de la Croix-Rouge"}
                }
            };

            //Act
            var result = (CreatedWithETagResult)await _controller.Propose(ResponseOptions, _idempotencyContext, _syndicationContext, mockPersistentLocalIdGenerator.Object, body);

            //Assert
            var expectedPosition = 1;
            result.Location.Should().Be($"https://www.registry.com/streetname/voorgesteld/{expectedLocation}");
            result.ETag.Should().Be(expectedPosition.ToString());
        }
    }
}
