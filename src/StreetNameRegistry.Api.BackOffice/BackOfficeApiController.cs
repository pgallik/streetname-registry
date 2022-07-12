namespace StreetNameRegistry.Api.BackOffice
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;

    public abstract class BackOfficeApiController : ApiController
    {
        protected IDictionary<string, object> GetMetadata()
        {
            var userId = User.FindFirst("urn:be:vlaanderen:streetnameregistry:acmid")?.Value;
            var correlationId = User.FindFirst(AddCorrelationIdMiddleware.UrnBasisregistersVlaanderenCorrelationId)?.Value;

            return new Dictionary<string, object>
            {
                { "UserId", userId },
                { "CorrelationId", correlationId }
            };
        }
    }
}
