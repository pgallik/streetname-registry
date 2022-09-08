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
    using TicketingService.Abstractions;

    public class SqsStreetNameApproveLambdaHandler : SqsLambdaHandler<SqsLambdaStreetNameApproveRequest>
    {
        public SqsStreetNameApproveLambdaHandler(
            IConfiguration configuration,
            ITicketing ticketing,
            IMunicipalities municipalities,
            IIdempotentCommandHandler idempotentCommandHandler)
            : base(
                configuration,
                municipalities,
                ticketing,
                idempotentCommandHandler)
        { }

        protected override async Task<ETagResponse> InnerHandle(SqsLambdaStreetNameApproveRequest request, CancellationToken cancellationToken)
        {
            var streetNamePersistentLocalId = new PersistentLocalId(request.Request.PersistentLocalId);
            var cmd = request.ToCommand(streetNamePersistentLocalId);

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
                StreetNameHasInvalidStatusException => new TicketError(
                    ValidationErrorMessages.StreetName.StreetNameCannotBeApproved,
                    ValidationErrorCodes.StreetName.StreetNameCannotBeApproved),
                MunicipalityHasInvalidStatusException => new TicketError(
                    ValidationErrorMessages.Municipality.MunicipalityStatusNotCurrent,
                    ValidationErrorCodes.Municipality.MunicipalityStatusNotCurrent),
                _ => null
            };
        }
    }
}
