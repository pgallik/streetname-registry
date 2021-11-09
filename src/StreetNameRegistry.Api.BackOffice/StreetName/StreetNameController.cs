namespace StreetNameRegistry.Api.BackOffice.StreetName
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Oslo.Extensions;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Convertors;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Requests;
    using Infrastructure.Options;
    using Microsoft.EntityFrameworkCore;
    using NodaTime.Extensions;
    using Projections.Syndication;
    using StreetNameRegistry.StreetName;
    using Swashbuckle.AspNetCore.Filters;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("straatnamen")]
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    public class StreetNameController : ApiBusController
    {
        public StreetNameController(ICommandHandlerResolver bus) : base(bus) {}

        /// <summary>
        /// Stel een straatnaam voor.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="idempotencyContext"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="persistentLocalIdGenerator"></param>
        /// <param name="streetNameProposeRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="201">Als de straatnaam voorgesteld is.</response>
        /// <response code="202">Als de straatnaam reeds voorgesteld is.</response>
        /// <returns></returns>
        [HttpPost("voorgesteld")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "location", "string", "De url van de voorgestelde straatnaam.", "")]
        [SwaggerRequestExample(typeof(StreetNameProposeRequest), typeof(StreetNameProposeRequestExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Propose(
            [FromServices] IOptions<ResponseOptions> options,
            [FromServices] IdempotencyContext idempotencyContext,
            [FromServices] SyndicationContext syndicationContext,
            [FromServices] IPersistentLocalIdGenerator persistentLocalIdGenerator,
            [FromBody] StreetNameProposeRequest streetNameProposeRequest,
            CancellationToken cancellationToken = default)
        {
            try
            {
                //TODO REMOVE WHEN IMPLEMENTED
                //return new CreatedWithETagResult(new Uri(string.Format(options.Value.DetailUrl, "1")), "1");
                //TODO real data please
                var fakeProvenanceData = new Provenance(
                        DateTime.UtcNow.ToInstant(),
                        Application.StreetNameRegistry,
                        new Reason(""),
                        new Operator(""),
                        Modification.Insert,
                        Organisation.DigitaalVlaanderen
                    );

                var identifier = streetNameProposeRequest.GemeenteId
                    .AsIdentifier()
                    .Map(IdentifierMappings.MunicipalityNisCode);

                var municipality = await syndicationContext.MunicipalityLatestItems
                    .AsNoTracking()
                    .SingleOrDefaultAsync(i =>
                            i.NisCode == identifier.Value, cancellationToken);

                var persistentLocalId = persistentLocalIdGenerator.GenerateNextPersistentLocalId();
                var cmd = streetNameProposeRequest.ToCommand(new MunicipalityId(municipality.MunicipalityId), fakeProvenanceData, persistentLocalId);
                var position = await IdempotentCommandHandlerDispatch(idempotencyContext, cmd.CreateCommandId(), cmd , cancellationToken);

                return new CreatedWithETagResult(new Uri(string.Format(options.Value.DetailUrl, persistentLocalId)), position.ToString(CultureInfo.InvariantCulture));
            }
            catch (IdempotencyException)
            {
                return Accepted();
            }
        }
    }
}
