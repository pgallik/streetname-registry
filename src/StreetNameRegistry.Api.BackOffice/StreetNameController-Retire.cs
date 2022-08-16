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
        /// Hef een straatnaam op.
        /// </summary>
        /// <param name="ifMatchHeaderValidator"></param>
        /// <param name="validator"></param>
        /// <param name="options"></param>
        /// <param name="request"></param>
        /// <param name="ifMatchHeaderValue"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Aanvraag tot opheffing wordt reeds verwerkt.</response>
        /// <response code="400">Als de straatnaam status niet 'inGebruik' is.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <returns></returns>
        [HttpPost("{persistentLocalId}/acties/opheffen")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Retire(
            [FromServices] IIfMatchHeaderValidator ifMatchHeaderValidator,
            [FromServices] IValidator<StreetNameRetireRequest> validator,
            [FromServices] IOptions<ResponseOptions> options,
            [FromRoute] StreetNameRetireRequest request,
            [FromHeader(Name = "If-Match")] string? ifMatchHeaderValue,
            CancellationToken cancellationToken = default)
        {
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
                    StreetNameNotFoundException => new ApiException("Onbestaande straatnaam.", StatusCodes.Status404NotFound),

                    StreetNameWasRemovedException => new ApiException("Verwijderde straatnaam.", StatusCodes.Status410Gone),

                    StreetNameStatusPreventsRetiringException => CreateValidationException(
                        "StraatnaamVoorgesteldOfAfgekeurd",
                        string.Empty,
                        "Deze actie is enkel toegestaan op straatnamen met status 'inGebruik'."),

                    MunicipalityHasUnexpectedStatusException _ => CreateValidationException(
                        "StraatnaamGemeenteInGebruik",
                        string.Empty,
                        "Deze actie is enkel toegestaan binnen gemeenten met status 'inGebruik'."),

                    _ => new ValidationException(new List<ValidationFailure>
                    {
                        new ValidationFailure(string.Empty, exception.Message)
                    })
                };
            }
        }
    }
}
