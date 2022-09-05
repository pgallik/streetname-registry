namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs
{
    using System.Collections.Generic;
    using Abstractions;
    using Requests;
    using TicketingService.Abstractions;

    public class SqsStreetNameRetireHandler : SqsHandler<SqsStreetNameRetireRequest>
    {
        public const string Action = "RetireStreetName";

        private readonly BackOfficeContext _backOfficeContext;

        public SqsStreetNameRetireHandler(
            ISqsQueue sqsQueue,
            ITicketing ticketing,
            ITicketingUrl ticketingUrl,
            BackOfficeContext backOfficeContext)
            : base (sqsQueue, ticketing, ticketingUrl)
        {
            _backOfficeContext = backOfficeContext;
        }

        protected override string? WithAggregateId(SqsStreetNameRetireRequest request)
        {
            var municipalityIdByPersistentLocalId = _backOfficeContext
                .MunicipalityIdByPersistentLocalId
                .Find(request.Request.PersistentLocalId);

            return municipalityIdByPersistentLocalId?.MunicipalityId.ToString();
        }

        protected override string WithDeduplicationId(string aggregateId, SqsStreetNameRetireRequest request)
        {
            throw new System.NotImplementedException();
        }

        protected override IDictionary<string, string> WithMetadata(string aggregateId, SqsStreetNameRetireRequest sqsRequest)
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
