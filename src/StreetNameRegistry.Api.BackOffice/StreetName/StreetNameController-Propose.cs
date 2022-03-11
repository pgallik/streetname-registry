namespace StreetNameRegistry.Api.BackOffice.StreetName
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Oslo.Extensions;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Consumer;
    using Convertors;
    using FluentValidation;
    using FluentValidation.Results;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Municipality;
    using Municipality.Exceptions;
    using Requests;
    using Swashbuckle.AspNetCore.Filters;

    public partial class StreetNameController
    {
        /// <summary>
        /// Stel een straatnaam voor.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="idempotencyContext"></param>
        /// <param name="consumerContext"></param>
        /// <param name="persistentLocalIdGenerator"></param>
        /// <param name="municipalityRepository"></param>
        /// <param name="streetNameProposeRequest"></param>
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
        [SwaggerResponseHeader(StatusCodes.Status201Created, "location", "string",
            "De url van de voorgestelde straatnaam.")]
        [SwaggerRequestExample(typeof(StreetNameProposeRequest), typeof(StreetNameProposeRequestExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Propose(
            [FromServices] IOptions<ResponseOptions> options,
            [FromServices] IdempotencyContext idempotencyContext,
            [FromServices] ConsumerContext consumerContext,
            [FromServices] BackOfficeContext backOfficeContext,
            [FromServices] IPersistentLocalIdGenerator persistentLocalIdGenerator,
            [FromServices] IValidator<StreetNameProposeRequest> validator,
            [FromServices] IMunicipalities municipalityRepository,
            [FromBody] StreetNameProposeRequest streetNameProposeRequest,
            CancellationToken cancellationToken = default)
        {
            await validator.ValidateAndThrowAsync(streetNameProposeRequest, cancellationToken);

            try
            {
                var fakeProvenanceData = new Provenance(
                    NodaTime.SystemClock.Instance.GetCurrentInstant(),
                    Application.StreetNameRegistry,
                    new Reason(""), // TODO: TBD
                    new Operator(""), // TODO: from claims
                    Modification.Insert,
                    Organisation.DigitaalVlaanderen // TODO: from claims
                );

                var identifier = streetNameProposeRequest.GemeenteId
                    .AsIdentifier()
                    .Map(IdentifierMappings.MunicipalityNisCode);

                var municipality = await consumerContext.MunicipalityConsumerItems
                    .AsNoTracking()
                    .SingleOrDefaultAsync(item =>
                        item.NisCode == identifier.Value, cancellationToken);
                if (municipality == null)
                {
                    throw new InvalidOperationException();
                }

                var persistentLocalId = persistentLocalIdGenerator.GenerateNextPersistentLocalId();
                var municipalityId = new MunicipalityId(municipality.MunicipalityId);
                
                var cmd = streetNameProposeRequest.ToCommand(municipalityId, fakeProvenanceData, persistentLocalId);
                await IdempotentCommandHandlerDispatch(idempotencyContext, cmd.CreateCommandId(), cmd,
                    cancellationToken);

                // Insert PersistentLocalId with MunicipalityId
                await backOfficeContext
                    .MunicipalityIdByPersistentLocalId
                    .AddAsync(new MunicipalityIdByPersistentLocalId(persistentLocalId, municipalityId), cancellationToken);
                await backOfficeContext.SaveChangesAsync(cancellationToken);

                var streetNameHash = await GetStreetNameHash(municipalityRepository, municipalityId, persistentLocalId, cancellationToken);
                return new CreatedWithLastObservedPositionAsETagResult(
                    new Uri(string.Format(options.Value.DetailUrl, persistentLocalId)), streetNameHash);
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
                        "StreetNameAlreadyExistsInMunicipality",
                        nameof(streetNameProposeRequest.Straatnamen),
                        $"Streetname '{nameExists.Name}' already exists within the municipality."),

                    MunicipalityWasRetiredException _ => CreateValidationException(
                        "StreetNameMunicipalityRetired",
                        nameof(streetNameProposeRequest.GemeenteId),
                        "This municipality was retired."),

                    StreetNameNameLanguageNotSupportedException _ => CreateValidationException(
                        "StreetNameLanguageNotOfficialOrFacilityLanguage",
                        nameof(streetNameProposeRequest.Straatnamen),
                        "'Straatnamen' can only be in the official or facility language of the municipality."),

                    StreetNameMissingLanguageException _ => CreateValidationException(
                        "StreetNameMissingOfficialOrFacilityLanguage",
                        nameof(streetNameProposeRequest.Straatnamen),
                        "'Straatnamen' is missing an official or facility language."),

                    _ => new ValidationException(new List<ValidationFailure>
                        {new ValidationFailure(string.Empty, exception.Message)})
                };
            }
        }
    }
}
