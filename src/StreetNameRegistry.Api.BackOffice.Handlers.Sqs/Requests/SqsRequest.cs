namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Newtonsoft.Json;

    public class SqsRequest : IRequest<LocationResult>
    {
        [JsonIgnore]
        public IDictionary<string, object> Metadata { get; set; }

        public Guid TicketId { get; set; }
    }

    public record LocationResult(string Location)
    {
        public Uri LocationAsUri => new Uri(Location);
    }
}
