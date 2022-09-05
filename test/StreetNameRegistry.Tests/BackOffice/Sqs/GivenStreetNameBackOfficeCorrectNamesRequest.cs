namespace StreetNameRegistry.Tests.BackOffice.Sqs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.MessageHandling.AwsSqs.Simple;
    using FluentAssertions;
    using global::AutoFixture;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Municipality;
    using StreetNameRegistry.Api.BackOffice.Abstractions;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Exceptions;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests;
    using Testing;
    using TicketingService.Abstractions;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenStreetNameBackOfficeCorrectNamesRequest : StreetNameRegistryTest
    {
        private readonly TestBackOfficeContext _backOfficeContext;

        public GivenStreetNameBackOfficeCorrectNamesRequest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new WithFixedPersistentLocalId());

            _backOfficeContext = new FakeBackOfficeContextFactory().CreateDbContext();
        }

        [Fact]
        public async Task ThenTicketWithLocationIsCreated()
        {
            // Arrange
            var municipalityLatestItem = _backOfficeContext.AddMunicipalityIdByPersistentLocalIdToFixture(
                Fixture.Create<PersistentLocalId>(),
                Fixture.Create<MunicipalityId>());

            var ticketId = Fixture.Create<Guid>();
            var ticketingMock = new Mock<ITicketing>();
            ticketingMock
                .Setup(x => x.CreateTicket(It.IsAny<IDictionary<string, string>>(), CancellationToken.None))
                .ReturnsAsync(ticketId);

            var ticketingUrl = new TicketingUrl(Fixture.Create<Uri>().ToString());

            var sqsQueue = new Mock<ISqsQueue>();

            var sut = new SqsStreetNameCorrectNamesHandler(
                sqsQueue.Object,
                ticketingMock.Object,
                ticketingUrl,
                _backOfficeContext);

            var sqsRequest = new SqsStreetNameCorrectNamesRequest
            {
                Request = new StreetNameBackOfficeCorrectNamesRequest
                {
                    PersistentLocalId = Fixture.Create<PersistentLocalId>(),
                    Straatnamen = new Dictionary<Taal, string>()
                    {
                        { Taal.NL, "Boskapelstraat" }
                    }
                }
            };

            // Act
            var result = (AcceptedResult)await sut.Handle(sqsRequest, CancellationToken.None);

            // Assert
            sqsRequest.TicketId.Should().Be(ticketId);
            sqsQueue.Verify(x => x.Copy(
                sqsRequest,
                It.Is<SqsQueueOptions>(y => y.MessageGroupId == municipalityLatestItem.MunicipalityId.ToString("D")),
                CancellationToken.None));
            result.Location.Should().Be(ticketingUrl.For(ticketId));
        }

        [Fact]
        public void WithNoMunicipalityFoundByStreetNamePersistentLocalId_ThrowsAggregateIdNotFound()
        {
            // Arrange
            var sut = new SqsStreetNameCorrectNamesHandler(
                Mock.Of<ISqsQueue>(),
                Mock.Of<ITicketing>(),
                Mock.Of<ITicketingUrl>(),
                _backOfficeContext);

            // Act
            var act = async () => await sut.Handle(
                new SqsStreetNameCorrectNamesRequest
                {
                    Request = Fixture.Create<StreetNameBackOfficeCorrectNamesRequest>()
                }, CancellationToken.None);

            // Assert
            act
                .Should()
                .ThrowAsync<AggregateIdNotFound>();
        }
    }
}
