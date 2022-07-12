namespace StreetNameRegistry.Api.BackOffice
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Exceptions;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using FluentValidation;
    using FluentValidation.Results;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Municipality.Exceptions;
    using Swashbuckle.AspNetCore.Filters;

    public partial class StreetNameController
    {
        /// <summary>
        /// Stel een straatnaam voor.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="request"></param>
        /// <param name="validator"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="201">Als de straatnaam voorgesteld is.</response>
        /// <response code="202">Als de straatnaam reeds voorgesteld is.</response>
        /// <returns></returns>
        [HttpPost("voorgesteld")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "location", "string", "De url van de voorgestelde straatnaam.")]
        [SwaggerRequestExample(typeof(StreetNameProposeRequest), typeof(StreetNameProposeRequestExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Propose(
            [FromServices] IOptions<ResponseOptions> options,
            [FromServices] IValidator<StreetNameProposeRequest> validator,
            [FromBody] StreetNameProposeRequest request,
            CancellationToken cancellationToken = default)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            try
            {
                request.Metadata = GetMetadata();
                var response = await _mediator.Send(request, cancellationToken);

                return new CreatedWithLastObservedPositionAsETagResult(
                    new Uri(string.Format(options.Value.DetailUrl, response.PersistentLocalId)),
                    response.LastEventHash);
            }
            catch (AggregateNotFoundException)
            {
                // todo: change this?
                throw CreateValidationException(
                    "code",
                    string.Empty,
                    "message");
            }
            catch (IdempotencyException)
            {
                return Accepted();
            }
            catch (DomainException exception)
            {
                throw exception switch
                {
                    StreetNameNameAlreadyExistsException nameExists => CreateValidationException(
                        "StraatnaamBestaatReedsInGemeente",
                        nameof(request.Straatnamen),
                        $"Straatnaam '{nameExists.Name}' bestaat reeds in de gemeente."),

                    MunicipalityHasUnexpectedStatusException _ => CreateValidationException(
                        "StraatnaamGemeenteGehistoreerd",
                        nameof(request.GemeenteId),
                        "De gemeente is gehistoreerd."),

                    StreetNameNameLanguageNotSupportedException _ => CreateValidationException(
                        "StraatnaamTaalNietInOfficieleOfFaciliteitenTaal",
                        nameof(request.Straatnamen),
                        "'Straatnamen' kunnen enkel voorkomen in de officiële of faciliteitentaal van de gemeente."),

                    StreetNameMissingLanguageException _ => CreateValidationException(
                        "StraatnaamOntbreektOfficieleOfFaciliteitenTaal",
                        nameof(request.Straatnamen),
                        "In 'Straatnamen' ontbreekt een officiële of faciliteitentaal."),

                    _ => new ValidationException(new List<ValidationFailure>
                        {new ValidationFailure(string.Empty, exception.Message)})
                };
            }
        }
    }
}
