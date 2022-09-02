namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs
{
    using System;
    using System.Linq;
    using Abstractions.Convertors;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Oslo.Extensions;
    using Be.Vlaanderen.Basisregisters.MessageHandling.AwsSqs.Simple;
    using Consumer;
    using Microsoft.EntityFrameworkCore;
    using Requests;
    using TicketingService.Abstractions;

    public class SqsStreetNameProposeHandler : SqsHandler<SqsStreetNameProposeRequest>
    {
        private readonly ConsumerContext _consumerContext;

        public SqsStreetNameProposeHandler(
            SqsOptions sqsOptions,
            ITicketing ticketing,
            ITicketingUrl ticketingUrl,
            ConsumerContext consumerContext)
            : base(sqsOptions, ticketing, ticketingUrl)
        {
            _consumerContext = consumerContext;
        }

        protected override string WithGroupId(SqsStreetNameProposeRequest request)
        {
            var identifier = request.Request.GemeenteId
                .AsIdentifier()
                .Map(IdentifierMappings.MunicipalityNisCode);

            var municipality = _consumerContext.MunicipalityConsumerItems
                .AsNoTracking()
                .SingleOrDefault(item => item.NisCode == identifier.Value);

            if (municipality == null)
            {
                throw new InvalidOperationException();
            }

            return municipality.MunicipalityId.ToString();
        }
    }
}
