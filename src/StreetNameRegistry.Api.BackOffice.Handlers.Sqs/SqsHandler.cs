namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.MessageHandling.AwsSqs.Simple;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Requests;
    using TicketingService.Abstractions;
    using static Be.Vlaanderen.Basisregisters.MessageHandling.AwsSqs.Simple.Sqs;
    using static Microsoft.AspNetCore.Http.Results;

    public abstract class SqsHandler<TSqsRequest> : IRequestHandler<TSqsRequest, IResult>
        where TSqsRequest : SqsRequest
    {
        private readonly SqsOptions _sqsOptions;
        private readonly ITicketing _ticketing;
        private readonly ITicketingUrl _ticketingUrl;

        protected SqsHandler(
            SqsOptions sqsOptions,
            ITicketing ticketing,
            ITicketingUrl ticketingUrl)
        {
            _sqsOptions = sqsOptions;
            _ticketing = ticketing;
            _ticketingUrl = ticketingUrl;
        }

        protected abstract string WithGroupId(TSqsRequest request);

        public async Task<IResult> Handle(TSqsRequest request, CancellationToken cancellationToken)
        {
            var ticketId = await _ticketing.CreateTicket(nameof(StreetNameRegistry), cancellationToken);
            request.TicketId = ticketId;

            var groupId = WithGroupId(request);

            if (string.IsNullOrEmpty(groupId))
            {
                throw new InvalidOperationException("No groupId.");
            }

            _ = await CopyToQueue(_sqsOptions, SqsQueueName.Value, request, new SqsQueueOptions { MessageGroupId = groupId }, cancellationToken);

            //_logger.LogDebug($"Request sent to queue {SqsQueueName.Value}");

            var location = _ticketingUrl.For(request.TicketId);
            return Accepted(location);
        }
    }
}
