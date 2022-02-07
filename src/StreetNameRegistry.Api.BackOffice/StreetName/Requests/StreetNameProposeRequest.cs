namespace StreetNameRegistry.Api.BackOffice.StreetName.Requests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Convertors;
    using FluentValidation;
    using Newtonsoft.Json;
    using StreetNameRegistry.StreetName;
    using StreetNameRegistry.StreetName.Commands;
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
        /// De straatnaam in elke officiële taal en faciliteitentaal van de gemeente.
        /// </summary>
        [DataMember(Name = "Straatnamen", Order = 2)]
        [JsonProperty(Required = Required.Always)]
        public Dictionary<Taal, string> Straatnamen { get; set; }

        /// <summary>
        /// Map to ProposeStreetName command
        /// </summary>
        /// <returns>ProposeStreetName.</returns>
        public ProposeStreetName ToCommand(MunicipalityId municipalityId, Provenance provenance, PersistentLocalId persistentLocalId)
        {
            var names = new Names(Straatnamen.Select(x => new StreetNameName(x.Value, x.Key.ToLanguage())));
            return new ProposeStreetName(municipalityId, names, persistentLocalId, provenance);
        }
    }

    public class StreetNameProposeRequestValidator : AbstractValidator<StreetNameProposeRequest>
    {
        public StreetNameProposeRequestValidator()
        {
            RuleForEach(x => x.Straatnamen)
                .Must(NotBeAnEmptyName)
                .WithMessage((_, streetName) => $"The streetname in '{streetName.Key.ToString().ToLowerInvariant()}' can not be empty."); ;

            RuleForEach(x => x.Straatnamen)
                .Must(HaveANameLength)
                .WithMessage((_, streetName) => $"The max length of a streetname in '{streetName.Key.ToString().ToLowerInvariant()}' is 60 characters. You currently have {streetName.Value.Length} characters.");
        }

        private static bool NotBeAnEmptyName(KeyValuePair<Taal, string> streetName) => !string.IsNullOrWhiteSpace(streetName.Value);

        private static bool HaveANameLength(KeyValuePair<Taal, string> streetName) => streetName.Value.Length <= 60;
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
