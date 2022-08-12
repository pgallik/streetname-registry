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
    using Infrastructure;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Municipality;
    using Municipality.Exceptions;
    using Swashbuckle.AspNetCore.Filters;

    public partial class StreetNameController
    {
        /// <summary>
        /// Corrigeer een straatnaam - straatnaam.
        /// </summary>
        /// <param name="ifMatchHeaderValidator"></param>
        /// <param name="validator"></param>
        /// <param name="options"></param>
        /// <param name="persistentLocalId"></param>
        /// <param name="request"></param>
        /// <param name="ifMatchHeaderValue"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Aanvraag tot correctie wordt reeds verwerkt.</response>
        /// <response code="400">Als de straatnaam status niet 'voorgesteld' of 'inGebruik' is.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <returns></returns>
        [HttpPost("{persistentLocalId}/acties/corrigeren/straatnaam")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> CorrectStreetNameNames(
            [FromServices] IIfMatchHeaderValidator ifMatchHeaderValidator,
            [FromServices] IValidator<StreetNameCorrectNamesRequest> validator,
            [FromServices] IOptions<ResponseOptions> options,
            [FromRoute] int persistentLocalId,
            [FromBody] StreetNameCorrectNamesRequest request,
            [FromHeader(Name = "If-Match")] string? ifMatchHeaderValue,
            CancellationToken cancellationToken = default)
        {
            request.PersistentLocalId = persistentLocalId;

            await validator.ValidateAndThrowAsync(request, cancellationToken);

            try
            {
                if (!await ifMatchHeaderValidator.IsValid(ifMatchHeaderValue, new PersistentLocalId(request.PersistentLocalId), cancellationToken))
                {
                    return new PreconditionFailedResult();
                }

                request.Metadata = GetMetadata();
                var response = await _mediator.Send(request, cancellationToken);

                return new AcceptedWithETagResult(
                    new Uri(string.Format(options.Value.DetailUrl, request.PersistentLocalId)),
                    response.LastEventHash);
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

                    StreetNameNotFoundException => new ApiException("Onbestaande straatnaam.", StatusCodes.Status404NotFound),

                    StreetNameWasRemovedException => new ApiException("Verwijderde straatnaam.", StatusCodes.Status410Gone),

                    StreetNameStatusPreventsCorrectingStreetNameNameException => CreateValidationException(
                        "StraatnaamGehistoreerdOfAfgekeurd",
                        String.Empty, 
                        "Deze actie is enkel toegestaan op straatnamen met status 'voorgesteld' of 'inGebruik'."),

                    StreetNameNameLanguageNotSupportedException _ => CreateValidationException(
                        "StraatnaamTaalNietInOfficieleOfFaciliteitenTaal",
                        nameof(request.Straatnamen),
                        "'Straatnamen' kunnen enkel voorkomen in de officiële of faciliteitentaal van de gemeente."),

                    _ => new ValidationException(new List<ValidationFailure>
                    {
                        new ValidationFailure(string.Empty, exception.Message)
                    })
                };
            }
        }
    }
}
