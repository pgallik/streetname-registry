namespace StreetNameRegistry.Api.BackOffice.StreetName
{
    using System;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Requests;
    using Infrastructure.Options;
    using Swashbuckle.AspNetCore.Filters;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("straatnamen")]
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    public class StreetNameController : ApiController
    {
        /// <summary>
        /// Stel een straatnaam voor.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="streetNameProposeRequest"></param>
        /// <response code="201">Als de straatnaam voorgesteld is.</response>
        /// <returns></returns>
        [HttpPost("voorgesteld")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "location", "string",
            "De url van de voorgestelde straatnaam.", "")]
        [SwaggerRequestExample(typeof(StreetNameProposeRequest), typeof(StreetNameProposeRequestExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Propose(
            [FromServices] IOptions<ResponseOptions> options,
            [FromBody] StreetNameProposeRequest streetNameProposeRequest)
        {

            return new CreatedWithETagResult(new Uri(string.Format(options.Value.DetailUrl, "1")), null, "123");
        }
    }
}
