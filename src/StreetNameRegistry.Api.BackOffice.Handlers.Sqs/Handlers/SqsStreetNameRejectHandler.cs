namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Handlers
{
    using System.Collections.Generic;
    using Abstractions;
    using Be.Vlaanderen.Basisregisters.Sqs;
    using Be.Vlaanderen.Basisregisters.Sqs.Handlers;
    using Requests;
    using TicketingService.Abstractions;

    public sealed class SqsStreetNameRejectHandler : SqsHandler<SqsStreetNameRejectRequest>
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

        protected override IDictionary<string, string> WithTicketMetadata(string aggregateId, SqsStreetNameRejectRequest sqsRequest)
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
