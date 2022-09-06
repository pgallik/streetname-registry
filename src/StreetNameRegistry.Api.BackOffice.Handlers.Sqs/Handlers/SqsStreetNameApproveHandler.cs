namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Handlers
{
    using System.Collections.Generic;
    using Abstractions;
    using Requests;
    using TicketingService.Abstractions;

    public class SqsStreetNameApproveHandler : SqsHandler<SqsStreetNameApproveRequest>
    {
        public const string Action = "ApproveStreetName";

        private readonly BackOfficeContext _backOfficeContext;

        public SqsStreetNameApproveHandler(
            ISqsQueue sqsQueue,
            ITicketing ticketing,
            ITicketingUrl ticketingUrl,
            BackOfficeContext backOfficeContext)
            : base (sqsQueue, ticketing, ticketingUrl)
        {
            _backOfficeContext = backOfficeContext;
        }

        protected override string? WithAggregateId(SqsStreetNameApproveRequest request)
        {
            var municipalityIdByPersistentLocalId = _backOfficeContext
                .MunicipalityIdByPersistentLocalId
                .Find(request.Request.PersistentLocalId);

            return municipalityIdByPersistentLocalId?.MunicipalityId.ToString();
        }

        protected override string WithDeduplicationId(string aggregateId, SqsStreetNameApproveRequest request)
        {
            throw new System.NotImplementedException();
        }

        protected override IDictionary<string, string> WithMetadata(string aggregateId, SqsStreetNameApproveRequest sqsRequest)
        {
            return new Dictionary<string, string>
            {
                { RegistryKey, nameof(StreetNameRegistry) },
                { ActionKey, Action },
                { AggregateIdKey, aggregateId },
                { ObjectIdKey, sqsRequest.Request.PersistentLocalId.ToString() }
            };
        }
    }
}
