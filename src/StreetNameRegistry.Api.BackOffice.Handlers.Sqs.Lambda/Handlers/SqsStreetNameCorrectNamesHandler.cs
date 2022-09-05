namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Municipality;
    using Municipality.Exceptions;
    using Requests;
    using TicketingService.Abstractions;
    using MunicipalityId = Municipality.MunicipalityId;

    public class SqsStreetNameCorrectNamesHandler : SqsLambdaHandler<SqsLambdaStreetNameCorrectNamesRequest>
    {
        private readonly IMunicipalities _municipalities;

        public SqsStreetNameCorrectNamesHandler(
            ITicketing ticketing,
            IMunicipalities municipalities,
            IIdempotentCommandHandler idempotentCommandHandler)
            : base(ticketing, idempotentCommandHandler)
        {
            _municipalities = municipalities;
        }

        protected override async Task<string> InnerHandle(SqsLambdaStreetNameCorrectNamesRequest request,
            CancellationToken cancellationToken)
        {
            var municipalityId = new MunicipalityId(Guid.Parse(request.MessageGroupId));
            var streetNamePersistentLocalId = new PersistentLocalId(request.Request.PersistentLocalId);

            var cmd = request.Request.ToCommand(
                municipalityId,
                CreateFakeProvenance());

            await IdempotentCommandHandler.Dispatch(
                cmd.CreateCommandId(),
                cmd,
                request.Metadata,
                cancellationToken);

            var lastEventHash = await GetStreetNameHash(_municipalities, municipalityId, streetNamePersistentLocalId,
                cancellationToken);

            return lastEventHash;
        }

        protected override TicketError? HandleDomainException(DomainException exception)
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
