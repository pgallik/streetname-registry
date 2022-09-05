namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class SqsRequest : IRequest<IActionResult>
    {
        [JsonIgnore]
        public IDictionary<string, object> Metadata { get; set; }

        public Guid TicketId { get; set; }
    }
}
