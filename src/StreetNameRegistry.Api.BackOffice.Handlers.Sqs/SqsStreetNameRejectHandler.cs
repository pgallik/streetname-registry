namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs
{
    using System.Collections.Generic;
    using Abstractions;
    using Requests;
    using TicketingService.Abstractions;

    public class SqsStreetNameRejectHandler : SqsHandler<SqsStreetNameRejectRequest>
    {
        public const string Action = "RejectStreetName";

        private readonly BackOfficeContext _backOfficeContext;

        public SqsStreetNameRejectHandler(
            ISqsQueue sqsQueue,
            ITicketing ticketing,
            ITicketingUrl ticketingUrl,
            BackOfficeContext backOfficeContext)
            : base (sqsQueue, ticketing, ticketingUrl)
        {
            _backOfficeContext = backOfficeContext;
        }

        protected override string? WithAggregateId(SqsStreetNameRejectRequest request)
        {
            var municipalityIdByPersistentLocalId = _backOfficeContext
                .MunicipalityIdByPersistentLocalId
                .Find(request.Request.PersistentLocalId);

            return municipalityIdByPersistentLocalId?.MunicipalityId.ToString();
        }

        protected override string WithDeduplicationId(string aggregateId, SqsStreetNameRejectRequest request)
        {
            throw new System.NotImplementedException();
        }

        protected override IDictionary<string, string> WithMetadata(string aggregateId, SqsStreetNameRejectRequest sqsRequest)
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
