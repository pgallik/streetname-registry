namespace StreetNameRegistry.Api.BackOffice.Abstractions.Requests;

using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
using Swashbuckle.AspNetCore.Filters;

public class StreetNameProposeRequestExamples : IExamplesProvider<StreetNameProposeRequest>
{
    public StreetNameProposeRequest GetExamples()
    {
        return new StreetNameProposeRequest
        {
            GemeenteId = "https://data.vlaanderen.be/id/gemeente/45041",
            Straatnamen = new Dictionary<Taal, string>
            {
                {Taal.NL, "Rodekruisstraat"},
                {Taal.FR, "Rue de la Croix-Rouge"}
            }
        };
    }
}