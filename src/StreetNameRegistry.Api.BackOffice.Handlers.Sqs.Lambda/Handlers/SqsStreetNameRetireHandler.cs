namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Municipality;
    using Municipality.Commands;
    using Municipality.Exceptions;
    using Requests;
    using TicketingService.Abstractions;
    using MunicipalityId = Municipality.MunicipalityId;

    public class SqsStreetNameRetireHandler : SqsLambdaHandler<SqsLambdaStreetNameRetireRequest>
    {
        private readonly IMunicipalities _municipalities;

        public SqsStreetNameRetireHandler(
            ITicketing ticketing,
            IMunicipalities municipalities,
            IIdempotentCommandHandler idempotentCommandHandler)
            : base(ticketing, idempotentCommandHandler)
        {
            _municipalities = municipalities;
        }

        protected override async Task<string> InnerHandle(SqsLambdaStreetNameRetireRequest request, CancellationToken cancellationToken)
        {
            var municipalityId = new MunicipalityId(Guid.Parse(request.MessageGroupId));
            var streetNamePersistentLocalId = new PersistentLocalId(request.Request.PersistentLocalId);

            var cmd = new RetireStreetName(
                municipalityId,
                streetNamePersistentLocalId,
                CreateFakeProvenance());

            await IdempotentCommandHandler.Dispatch(
                cmd.CreateCommandId(),
                cmd,
                request.Metadata,
                cancellationToken);

            var lastEventHash = await GetStreetNameHash(_municipalities, municipalityId, streetNamePersistentLocalId, cancellationToken);

            return lastEventHash;
        }

        protected override TicketError? HandleDomainException(DomainException exception)
        {
            return exception switch
            {
                StreetNameHasInvalidStatusException => new TicketError(
                    ValidationErrorMessages.StreetName.StreetNameCannotBeRetired,
                    ValidationErrorCodes.StreetName.StreetNameCannotBeRetired),
                MunicipalityHasInvalidStatusException => new TicketError(
                    ValidationErrorMessages.Municipality.MunicipalityStatusNotCurrent,
                    ValidationErrorCodes.Municipality.MunicipalityStatusNotCurrent),
                _ => null
            };
        }
    }
}
