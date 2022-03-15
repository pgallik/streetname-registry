namespace StreetNameRegistry.Api.BackOffice.StreetName
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Municipality;
    using Municipality.Commands;
    using Requests;
    using Municipality.Exceptions;
    using Swashbuckle.AspNetCore.Filters;

    public partial class StreetNameController
    {
        /// <summary>
        /// Keur een straatnaam goed.
        /// </summary>
        /// <param name="idempotencyContext"></param>
        /// <param name="municipalityRepository"></param>
        /// <param name="streetNameApproveRequest"></param>
        /// <param name="backOfficeContext"></param>
        /// <param name="validator"></param>
        /// <param name="ifMatchHeaderValue"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Aanvraag tot goedkeuring wordt reeds verwerkt.</response>
        /// <response code="204">Als de straatnaam goedgekeurd is.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <returns></returns>
        [HttpPut("{persistentLocalId}/goedgekeurd")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Approve(
            [FromServices] IdempotencyContext idempotencyContext,
            [FromServices] BackOfficeContext backOfficeContext,
            [FromServices] IValidator<StreetNameApproveRequest> validator,
            [FromServices] IMunicipalities municipalityRepository,
            [FromRoute] StreetNameApproveRequest streetNameApproveRequest,
            [FromHeader(Name = "If-Match")] string? ifMatchHeaderValue,
            CancellationToken cancellationToken = default)
        {
            await validator.ValidateAndThrowAsync(streetNameApproveRequest, cancellationToken);

            try
            {
                var fakeProvenanceData = new Provenance(
                    NodaTime.SystemClock.Instance.GetCurrentInstant(),
                    Application.StreetNameRegistry,
                    new Reason(""), // TODO: TBD
                    new Operator(""), // TODO: from claims
                    Modification.Update,
                    Organisation.DigitaalVlaanderen // TODO: from claims
                );

                var persistentLocalId = new PersistentLocalId(streetNameApproveRequest.PersistentLocalId);

                var municipalityIdByPersistentLocalId = await backOfficeContext
                    .MunicipalityIdByPersistentLocalId
                    .FindAsync(streetNameApproveRequest.PersistentLocalId);
                if (municipalityIdByPersistentLocalId is null)
                {
                    return NotFound();
                }

                var municipalityId = new MunicipalityId(municipalityIdByPersistentLocalId.MunicipalityId);

                // Check if user provided ETag is equal to the current Entity Tag
                if (ifMatchHeaderValue is not null)
                {
                    var ifMatchTag = ifMatchHeaderValue.Trim();
                    var lastHash = await GetStreetNameHash(municipalityRepository, municipalityId, persistentLocalId, cancellationToken);
                    var lastHashTag = new ETag(ETagType.Strong, lastHash);
                    if (ifMatchTag != lastHashTag.ToString())
                    {
                        return new PreconditionFailedResult();
                    }
                }

                var cmd = new ApproveStreetName(municipalityId, persistentLocalId, fakeProvenanceData);
                await IdempotentCommandHandlerDispatch(idempotencyContext, cmd.CreateCommandId(), cmd, cancellationToken);

                var etag = await GetStreetNameHash(municipalityRepository, municipalityId, persistentLocalId, cancellationToken);
                return new NoContentWithETagResult(etag);
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

                    StreetNameWasRemovedException => new ApiException("Straatnaam verwijderd.", StatusCodes.Status410Gone),

                    _ => new ValidationException(new List<ValidationFailure>
                    {
                        new ValidationFailure(string.Empty, exception.Message)
                    })
                };
            }
        }
    }
}
