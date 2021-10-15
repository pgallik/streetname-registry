namespace StreetNameRegistry.Api.BackOffice.StreetName.Requests
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;

    [DataContract(Name = "VoorstelStraatnaam", Namespace = "")]
    public class StreetNameProposeRequest
    {
        /// <summary>
        /// De unieke en persistente identificator van de gemeente die de straatnaam toekent.
        /// </summary>
        [DataMember(Name = "GemeenteId", Order = 1)]
        [JsonProperty(Required = Required.Always)]
        public string GemeenteId { get; set; }

        /// <summary>
        /// De naam van de straat in elke officiële taal en faciliteitentaal van de gemeente.
        /// </summary>
        [DataMember(Name = "Straatnamen", Order = 2)]
        [JsonProperty(Required = Required.Always)]
        public Dictionary<Taal, string> Straatnamen { get; set; }
    }

    public class StreetNameProposeRequestExamples : IExamplesProvider<StreetNameProposeRequest>
    {
        public StreetNameProposeRequest GetExamples()
        {
            return new StreetNameProposeRequest()
            {
                GemeenteId = "https://data.vlaanderen.be/id/gemeente/45041",
                Straatnamen = new Dictionary<Taal, string>()
                {
                    {Taal.NL, "Rodekruisstraat"},
                    {Taal.FR, "Rue de la Croix-Rouge"}
                }
            };
        }
    }
}
