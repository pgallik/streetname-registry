namespace StreetNameRegistry.Tests.BackOffice.Lambda
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Moq;
    using Municipality;
    using Municipality.Exceptions;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Response;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests;
    using TicketingService.Abstractions;
    using Xunit;
    using Xunit.Abstractions;

    public class SqsLambdaHandlerTests : BackOfficeTest
    {
        public SqsLambdaHandlerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task TicketShouldBeUpdatedToPendingAndCompleted()
        {
            var ticketing = new Mock<ITicketing>();
            var idempotentCommandHandler = new Mock<IIdempotentCommandHandler>();

            var sqsLambdaRequest = new SqsLambdaStreetNameProposeRequest
            {
                Request = new StreetNameBackOfficeProposeRequest(),
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid()
            };

            var sut = new FakeLambdaHandler(
                ticketing.Object,
                idempotentCommandHandler.Object);

            await sut.Handle(sqsLambdaRequest, CancellationToken.None);

            ticketing.Verify(x => x.Pending(sqsLambdaRequest.TicketId, CancellationToken.None), Times.Once);
            ticketing.Verify(x => x.Complete(sqsLambdaRequest.TicketId, new TicketResult(new ETagResponse("eTag")), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenStreetNameIsNotFoundException_ThenTicketingErrorIsExpected()
        {
            var ticketing = new Mock<ITicketing>();

            var sqsLambdaRequest = new SqsLambdaStreetNameProposeRequest
            {
                Request = new StreetNameBackOfficeProposeRequest(),
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid()
            };

            var sut = new FakeLambdaHandler(
                ticketing.Object,
                MockExceptionIdempotentCommandHandler<StreetNameIsNotFoundException>().Object);

            await sut.Handle(sqsLambdaRequest, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Error(sqsLambdaRequest.TicketId, new TicketError("Onbestaande straatnaam.", "OnbestaandeStraatnaam"), CancellationToken.None));
            ticketing.Verify(x => x.Complete(It.IsAny<Guid>(), It.IsAny<TicketResult>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task WhenAggregateNotFoundException_ThenTicketingErrorIsExpected()
        {
            var ticketing = new Mock<ITicketing>();

            var sqsLambdaRequest = new SqsLambdaStreetNameProposeRequest
            {
                Request = new StreetNameBackOfficeProposeRequest(),
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid()
            };

            var idempotentCommandHandler = new Mock<IIdempotentCommandHandler>();
            idempotentCommandHandler
                .Setup(x => x.Dispatch(It.IsAny<Guid>(), It.IsAny<object>(),
                    It.IsAny<IDictionary<string, object>>(), CancellationToken.None))
                .Throws(new AggregateNotFoundException("dummy", typeof(string)));

            var sut = new FakeLambdaHandler(
                ticketing.Object,
                idempotentCommandHandler.Object);

            await sut.Handle(sqsLambdaRequest, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Error(sqsLambdaRequest.TicketId, new TicketError("", ""), CancellationToken.None));
            ticketing.Verify(x => x.Complete(It.IsAny<Guid>(), It.IsAny<TicketResult>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task WhenStreetNameIsRemovedException_ThenTicketingErrorIsExpected()
        {
            var ticketing = new Mock<ITicketing>();

            var sqsLambdaRequest = new SqsLambdaStreetNameProposeRequest
            {
                Request = new StreetNameBackOfficeProposeRequest(),
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid()
            };

            var sut = new FakeLambdaHandler(
                ticketing.Object,
                MockExceptionIdempotentCommandHandler<StreetNameIsRemovedException>().Object);

            await sut.Handle(sqsLambdaRequest, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Error(sqsLambdaRequest.TicketId, new TicketError("Verwijderde straatnaam.", "VerwijderdeStraatnaam"), CancellationToken.None));
            ticketing.Verify(x => x.Complete(It.IsAny<Guid>(), It.IsAny<TicketResult>(), CancellationToken.None), Times.Never);
        }
    }

    public class FakeLambdaHandler : SqsLambdaHandler<SqsLambdaStreetNameProposeRequest>
    {
        public FakeLambdaHandler(ITicketing ticketing, IIdempotentCommandHandler idempotentCommandHandler) : base(ticketing, idempotentCommandHandler)
        {
        }

        protected override Task<string> InnerHandle(SqsLambdaStreetNameProposeRequest request, CancellationToken cancellationToken)
        {
            IdempotentCommandHandler.Dispatch(
                Guid.NewGuid(),
                new object(),
                new Dictionary<string, object>(),
                cancellationToken);

            return Task.FromResult("eTag");
        }

        protected override TicketError? HandleDomainException(DomainException exception)
        {
            throw new System.NotImplementedException();
        }
    }
}
