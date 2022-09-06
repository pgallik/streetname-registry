namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Handlers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Exceptions;
    using Be.Vlaanderen.Basisregisters.MessageHandling.AwsSqs.Simple;
    using MediatR;
    using Requests;
    using TicketingService.Abstractions;

    public abstract class SqsHandler<TSqsRequest> : IRequestHandler<TSqsRequest, LocationResult>
        where TSqsRequest : SqsRequest
    {
        public const string ActionKey = "Action";
        public const string RegistryKey = "Registry";
        public const string AggregateIdKey = "AggregateId";
        public const string ObjectIdKey = "ObjectId";

        private readonly ITicketing _ticketing;
        private readonly ITicketingUrl _ticketingUrl;
        private readonly ISqsQueue _sqsQueue;

        protected SqsHandler(
            ISqsQueue sqsQueue,
            ITicketing ticketing,
            ITicketingUrl ticketingUrl)
        {
            _sqsQueue = sqsQueue;
            _ticketing = ticketing;
            _ticketingUrl = ticketingUrl;
        }

        protected abstract string? WithAggregateId(TSqsRequest request);
        protected abstract IDictionary<string, string> WithMetadata(string aggregateId, TSqsRequest sqsRequest);

        public async Task<LocationResult> Handle(TSqsRequest request, CancellationToken cancellationToken)
        {
            var aggregateId = WithAggregateId(request);

            if (string.IsNullOrEmpty(aggregateId))
            {
                throw new AggregateIdNotFound();
            }

            var ticketId = await _ticketing.CreateTicket(WithMetadata(aggregateId, request), cancellationToken);
            request.TicketId = ticketId;

            _ = await _sqsQueue.Copy(request, new SqsQueueOptions { MessageGroupId = aggregateId }, cancellationToken);

            //_logger.LogDebug($"Request sent to queue {SqsQueueName.Value}");

            var location = _ticketingUrl.For(request.TicketId);

            return new LocationResult(location);
        }
    }
}
