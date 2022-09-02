namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Newtonsoft.Json;

    public class SqsLambdaRequest : IRequest
    {
        [JsonIgnore]
        public IDictionary<string, object> Metadata { get; set; }

        [JsonIgnore]
        public string? MessageGroupId { get; set; }

        public Guid TicketId { get; set; }
    }
}
