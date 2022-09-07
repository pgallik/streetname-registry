namespace StreetNameRegistry.Api.BackOffice.Abstractions.Requests
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using MediatR;
    using Newtonsoft.Json;
    using Response;

    [DataContract(Name = "OpheffenStraatnaam", Namespace = "")]
    public class StreetNameRetireRequest : StreetNameBackOfficeRetireRequest, IRequest<ETagResponse>
    {
        [JsonIgnore]
        public IDictionary<string, object> Metadata { get; set; }
    }
}
