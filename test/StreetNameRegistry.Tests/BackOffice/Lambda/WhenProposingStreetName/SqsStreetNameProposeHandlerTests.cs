namespace StreetNameRegistry.Tests.BackOffice.Lambda.WhenProposingStreetName
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentAssertions;
    using Moq;
    using Municipality;
    using SqlStreamStore;
    using SqlStreamStore.Streams;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Response;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers;
    using Xunit;
    using Xunit.Abstractions;

    public class SqsStreetNameProposeHandlerTests : BackOfficeTest
    {
        private readonly TestConsumerContext _consumerContext;
        private readonly TestBackOfficeContext _backOfficeContext;
        private readonly IdempotencyContext _idempotencyContext;

        public SqsStreetNameProposeHandlerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _consumerContext = new FakeConsumerContextFactory().CreateDbContext(Array.Empty<string>());
            _backOfficeContext = new FakeBackOfficeContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public async Task ThenTheStreetNameIsProposed()
        {
            const int expectedLocation = 5;

            //Arrange
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(expectedLocation));

            var municipalityId = new MunicipalityId(municipalityLatestItem.MunicipalityId);
            ImportMunicipality(municipalityId, new NisCode("23002"));
            AddOfficialLanguageDutch(municipalityId);
            AddOfficialLanguageFrench(municipalityId);

            ETagResponse? etag = null;

            var handler = new SqsStreetNameProposeHandler(
                MockTicketing(result =>
                {
                    etag = result;
                }).Object,
                MockTicketingUrl().Object,
                Container.Resolve<ICommandHandlerResolver>(),
                mockPersistentLocalIdGenerator.Object,
                _idempotencyContext,
                _backOfficeContext,
                Container.Resolve<IMunicipalities>());

            //Act
            await handler.Handle(new SqsStreetNameProposeRequest
            {
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{municipalityLatestItem.NisCode}",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "Rodekruisstraat" },
                    { Taal.FR, "Rue de la Croix-Rouge" }
                },
                MessageGroupId = municipalityId
            }, CancellationToken.None);

            //Assert
            var stream = await Container.Resolve<IStreamStore>().ReadStreamBackwards(new StreamId(new MunicipalityStreamId(municipalityId)), 3, 1); //3 = version of stream (zero based)
            stream.Messages.First().JsonMetadata.Should().Contain(etag.LastEventHash);

            var municipalityIdByPersistentLocalId = await _backOfficeContext.MunicipalityIdByPersistentLocalId.FindAsync(expectedLocation);
            municipalityIdByPersistentLocalId.Should().NotBeNull();
            municipalityIdByPersistentLocalId.MunicipalityId.Should().Be(municipalityLatestItem.MunicipalityId);
        }
    }
}
