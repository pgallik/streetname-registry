namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class SqsRequest : IRequest<IResult>
    {
        [JsonIgnore]
        public IDictionary<string, object> Metadata { get; set; }

        public Guid TicketId { get; set; }
    }
}
