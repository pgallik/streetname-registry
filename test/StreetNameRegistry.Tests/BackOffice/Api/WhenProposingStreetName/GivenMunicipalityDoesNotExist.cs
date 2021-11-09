namespace StreetNameRegistry.Tests.BackOffice.Api.WhenProposingStreetName
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentAssertions;
    using global::AutoFixture;
    using Infrastructure;
    using Moq;
    using StreetName;
    using StreetNameRegistry.Api.BackOffice.StreetName;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using Testing;
    using Xunit.Abstractions;

    public class GivenMunicipalityDoesNotExist : StreetNameRegistryBackOfficeTest
    {
        private readonly Fixture _fixture;
        private readonly StreetNameController _controller;
        private readonly TestSyndicationContext _syndicationContext;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityDoesNotExist(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _controller = CreateApiBusControllerWithUser<StreetNameController>("John Doe");
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _syndicationContext = new FakeSyndicationContextFactory().CreateDbContext(Array.Empty<string>());
        }

        //TODO: uncomment when implementation is done
        //[Fact]
        public async Task ThenAggregateIsNotFound()
        {
            //Arrange
            var municipalityLatestItem = _syndicationContext.AddMunicipalityLatestItemFixture();
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(5));
            
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
            Func<Task> act = async () => await _controller.Propose(ResponseOptions, _idempotencyContext, _syndicationContext, mockPersistentLocalIdGenerator.Object, body);

            //Assert
            act.Should().Throw<AggregateNotFoundException>();
        }
    }
}
