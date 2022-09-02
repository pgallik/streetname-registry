namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs
{
    using Abstractions;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.MessageHandling.AwsSqs.Simple;
    using Requests;
    using TicketingService.Abstractions;

    public class SqsStreetNameApproveHandler : SqsHandler<SqsStreetNameApproveRequest>
    {
        private readonly BackOfficeContext _backOfficeContext;

        public SqsStreetNameApproveHandler(
            SqsOptions sqsOptions,
            ITicketing ticketing,
            ITicketingUrl ticketingUrl,
            BackOfficeContext backOfficeContext)
            : base (sqsOptions, ticketing, ticketingUrl)
        {
            _backOfficeContext = backOfficeContext;
        }

        protected override string WithGroupId(SqsStreetNameApproveRequest request)
        {
            var municipalityIdByPersistentLocalId = _backOfficeContext
                .MunicipalityIdByPersistentLocalId
                .Find(request.Request.PersistentLocalId);

            return municipalityIdByPersistentLocalId.ToString();
        }
    }
}
