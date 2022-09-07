namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using MediatR;

    public class SqsLambdaRequest : IRequest
    {
        public IDictionary<string, object> Metadata { get; set; }
        public Provenance Provenance { get; set; }
        public string? MessageGroupId { get; set; }
        public Guid TicketId { get; set; }
    }
}
