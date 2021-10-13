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
        /// PURI met NIS-code van de gemeente die de straatnaam toekent.
        /// </summary>
        [DataMember(Name = "GemeenteId", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string GemeenteId { get; set; }

        /// <summary>
        /// Eén of twee taalstrings bestaande uit een straatnaam en de taal waarin deze wordt voorgesteld.
        /// </summary>
        [DataMember(Name = "Straatnamen", Order = 2)]
        [JsonProperty(Required = Required.DisallowNull)]
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
