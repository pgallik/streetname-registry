namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs
{
    using Abstractions;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.MessageHandling.AwsSqs.Simple;
    using TicketingService.Abstractions;

    public class SqsStreetNameRetireHandler : SqsHandler<SqsStreetNameRetireRequest>
    {
        private readonly BackOfficeContext _backOfficeContext;

        public SqsStreetNameRetireHandler(
            SqsOptions sqsOptions,
            ITicketing ticketing,
            ITicketingUrl ticketingUrl,
            BackOfficeContext backOfficeContext)
            : base (sqsOptions, ticketing, ticketingUrl)
        {
            _backOfficeContext = backOfficeContext;
        }

        protected override string WithGroupId(SqsStreetNameRetireRequest request)
        {
            var municipalityIdByPersistentLocalId = _backOfficeContext
                .MunicipalityIdByPersistentLocalId
                .Find(request.PersistentLocalId);

            return municipalityIdByPersistentLocalId.ToString();
        }
    }
}
