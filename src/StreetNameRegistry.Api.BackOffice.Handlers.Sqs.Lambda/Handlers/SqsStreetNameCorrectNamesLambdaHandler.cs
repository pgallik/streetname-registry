namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Exceptions;
    using Abstractions.Response;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Microsoft.Extensions.Configuration;
    using Municipality;
    using Municipality.Exceptions;
    using Requests;
    using StreetNameRegistry.Infrastructure;
    using TicketingService.Abstractions;

    public class SqsStreetNameCorrectNamesLambdaHandler : SqsLambdaHandler<SqsLambdaStreetNameCorrectNamesRequest>
    {
        public SqsStreetNameCorrectNamesLambdaHandler(
            IConfiguration configuration,
            ICustomRetryPolicy retryPolicy,
            ITicketing ticketing,
            IMunicipalities municipalities,
            IIdempotentCommandHandler idempotentCommandHandler)
            : base(
                configuration,
                retryPolicy,
                municipalities,
                ticketing,
                idempotentCommandHandler)
        { }

        protected override async Task<ETagResponse> InnerHandle(SqsLambdaStreetNameCorrectNamesRequest request, CancellationToken cancellationToken)
        {
            var streetNamePersistentLocalId = new PersistentLocalId(request.StreetNamePersistentLocalId);
            var cmd = request.ToCommand();

            try
            {
                await IdempotentCommandHandler.Dispatch(
                    cmd.CreateCommandId(),
                    cmd,
                    request.Metadata,
                    cancellationToken);
            }
            catch (IdempotencyException)
            {
                // Idempotent: Do Nothing return last etag
            }

            var lastHash = await GetStreetNameHash(request.MunicipalityId, streetNamePersistentLocalId, cancellationToken);
            return new ETagResponse(string.Format(DetailUrlFormat, streetNamePersistentLocalId), lastHash);
        }

        protected override TicketError? MapDomainException(DomainException exception)
        {
            return exception switch
            {
                StreetNameNameAlreadyExistsException nameExists => new TicketError(
                    ValidationErrorMessages.StreetName.StreetNameAlreadyExists(nameExists.Name),
                    ValidationErrorCodes.StreetName.StreetNameAlreadyExists),
                StreetNameHasInvalidStatusException => new TicketError(
                    ValidationErrorMessages.StreetName.StreetNameCannotBeCorrected,
                    ValidationErrorCodes.StreetName.StreetNameCannotBeCorrected),
                StreetNameNameLanguageIsNotSupportedException _ => new TicketError(
                    ValidationErrorMessages.StreetName.StreetNameNameLanguageIsNotSupported,
                    ValidationErrorCodes.StreetName.StreetNameNameLanguageIsNotSupported),
                _ => null
            };
        }
    }
}
