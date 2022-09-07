namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Municipality;
    using Municipality.Exceptions;
    using Requests;
    using TicketingService.Abstractions;

    public class SqsStreetNameProposeHandler : SqsLambdaHandler<SqsLambdaStreetNameProposeRequest>
    {
        private readonly IPersistentLocalIdGenerator _persistentLocalIdGenerator;
        private readonly BackOfficeContext _backOfficeContext;
        private readonly IMunicipalities _municipalities;

        public SqsStreetNameProposeHandler(
            ITicketing ticketing,
            IPersistentLocalIdGenerator persistentLocalIdGenerator,
            IIdempotentCommandHandler idempotentCommandHandler,
            BackOfficeContext backOfficeContext,
            IMunicipalities municipalities
            ) : base(ticketing, idempotentCommandHandler)
        {
            _persistentLocalIdGenerator = persistentLocalIdGenerator;
            _backOfficeContext = backOfficeContext;
            _municipalities = municipalities;
        }

        protected override async Task<string> InnerHandle(SqsLambdaStreetNameProposeRequest request, CancellationToken cancellationToken)
        {
            var persistentLocalId = _persistentLocalIdGenerator.GenerateNextPersistentLocalId();
            var municipalityId = new MunicipalityId(new Guid(request.MessageGroupId!));

            var cmd = request.ToCommand(
                municipalityId,
                persistentLocalId);

            await IdempotentCommandHandler.Dispatch(
                cmd.CreateCommandId(),
                cmd,
                request.Metadata,
                cancellationToken);

            // Insert PersistentLocalId with MunicipalityId
            await _backOfficeContext
                .MunicipalityIdByPersistentLocalId
                .AddAsync(new MunicipalityIdByPersistentLocalId(persistentLocalId, municipalityId), cancellationToken);
            await _backOfficeContext.SaveChangesAsync(cancellationToken);

            var streetNameHash = await GetStreetNameHash(_municipalities, municipalityId, persistentLocalId, cancellationToken);

            return streetNameHash;
        }

        protected override TicketError? HandleDomainException(DomainException exception)
        {
            return exception switch
            {
                StreetNameNameAlreadyExistsException nameExists => new TicketError(
                    ValidationErrorMessages.StreetName.StreetNameAlreadyExists(nameExists.Name),
                    ValidationErrorCodes.StreetName.StreetNameAlreadyExists),
                MunicipalityHasInvalidStatusException => new TicketError(
                    ValidationErrorMessages.Municipality.MunicipalityHasInvalidStatus,
                    ValidationErrorCodes.Municipality.MunicipalityHasInvalidStatus),
                StreetNameNameLanguageIsNotSupportedException _ => new TicketError(
                    ValidationErrorMessages.StreetName.StreetNameNameLanguageIsNotSupported,
                    ValidationErrorCodes.StreetName.StreetNameNameLanguageIsNotSupported),
                StreetNameIsMissingALanguageException _ => new TicketError(
                    ValidationErrorMessages.StreetName.StreetNameIsMissingALanguage,
                    ValidationErrorCodes.StreetName.StreetNameIsMissingALanguage),
                _ => null
            };
        }
    }
}
