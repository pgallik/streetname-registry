namespace StreetNameRegistry.Api.BackOffice.Abstractions.Requests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Convertors;
    using Municipality;
    using Municipality.Commands;
    using Newtonsoft.Json;

    [DataContract(Name = "CorrigerenStraatnaamNamen", Namespace = "")]
    public class SqsStreetNameCorrectNamesRequest : SqsRequest
    {
        /// <summary>
        /// De unieke en persistente identificator van de straat.
        /// </summary>
        [DataMember(Name = "PersistentLocalId", Order = 0)]
        public int PersistentLocalId { get; set; }

        /// <summary>
        /// De straatnaam in elke officiële taal en faciliteitentaal van de gemeente.
        /// </summary>
        [DataMember(Name = "Straatnamen", Order = 1)]
        [JsonProperty(Required = Required.Always)]
        public Dictionary<Taal, string> Straatnamen { get; set; }

        /// <summary>
        /// Map to CorrectStreetNameNames command
        /// </summary>
        /// <returns>CorrectStreetNameNames.</returns>
        public CorrectStreetNameNames ToCommand(MunicipalityId municipalityId, Provenance provenance)
        {
            var names = new Names(Straatnamen.Select(x => new StreetNameName(x.Value, x.Key.ToLanguage())));
            return new CorrectStreetNameNames(municipalityId, new PersistentLocalId(PersistentLocalId), names, provenance);
        }
    }
}
