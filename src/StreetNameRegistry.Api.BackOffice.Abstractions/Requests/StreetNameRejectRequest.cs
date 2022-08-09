namespace StreetNameRegistry.Api.BackOffice.Abstractions.Requests
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using MediatR;
    using Newtonsoft.Json;
    using Response;

    [DataContract(Name = "AfkeurenStraatnaam", Namespace = "")]
    public class StreetNameRejectRequest : IRequest<ETagResponse>
    {
        /// <summary>
        /// De unieke en persistente identificator van de straat.
        /// </summary>
        [DataMember(Name = "PersistentLocalId", Order = 0)]
        [JsonProperty(Required = Required.Always)]
        public int PersistentLocalId { get; set; }

        [JsonIgnore]
        public IDictionary<string, object> Metadata { get; set; }
    }
}
