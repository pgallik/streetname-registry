namespace StreetNameRegistry.Api.BackOffice
{
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
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Municipality;
    using Municipality.Exceptions;
    using Swashbuckle.AspNetCore.Filters;
    using Validators;

    public partial class StreetNameController
    {
        /// <summary>
        /// Keur een straatnaam goed.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ifMatchHeaderValidator"></param>
        /// <param name="validator"></param>
        /// <param name="ifMatchHeaderValue"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Aanvraag tot goedkeuring wordt reeds verwerkt.</response>
        /// <response code="204">Als de straatnaam goedgekeurd is.</response>
        /// <response code="409">Als de straatnaam status niet 'voorgesteld' is.</response>
        /// <response code="412">Als de If-Match header niet overeenkomt met de laatste ETag.</response>
        /// <returns></returns>
        [HttpPut("{persistentLocalId}/goedgekeurd")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Approve(
            [FromServices] IIfMatchHeaderValidator ifMatchHeaderValidator,
            [FromServices] IValidator<StreetNameApproveRequest> validator,
            [FromRoute] StreetNameApproveRequest request,
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

                return new NoContentWithETagResult(response.LastEventHash);
            }
            catch (IdempotencyException)
            {
                return Accepted();
            }
            catch (DomainException exception)
            {
                throw exception switch
                {
                    StreetNameIsNotFoundException => new ApiException(ValidationErrorMessages.StreetName.StreetNameNotFound, StatusCodes.Status404NotFound),

                    StreetNameIsRemovedException => new ApiException(ValidationErrorMessages.StreetName.StreetNameIsRemoved, StatusCodes.Status410Gone),

                    StreetNameHasInvalidStatusException => new ApiException(ValidationErrorMessages.StreetName.StreetNameCannotBeApproved, StatusCodes.Status409Conflict),

                    MunicipalityHasInvalidStatusException _ => CreateValidationException(
                        ValidationErrorCodes.Municipality.MunicipalityStatusNotCurrent,
                        string.Empty,
                        ValidationErrorMessages.Municipality.MunicipalityStatusNotCurrent),

                    _ => new ValidationException(new List<ValidationFailure>
                    {
                        new ValidationFailure(string.Empty, exception.Message)
                    })
                };
            }
        }
    }
}
