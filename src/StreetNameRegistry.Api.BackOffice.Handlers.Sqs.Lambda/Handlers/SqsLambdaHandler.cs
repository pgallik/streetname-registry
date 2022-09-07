namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using MediatR;
    using Abstractions;
    using Abstractions.Response;
    using Requests;
    using Municipality;
    using Municipality.Exceptions;
    using TicketingService.Abstractions;

    public abstract class SqsLambdaHandler<TSqsLambdaRequest> : IRequestHandler<TSqsLambdaRequest>
        where TSqsLambdaRequest : SqsLambdaRequest
    {
        private readonly ITicketing _ticketing;

        protected IIdempotentCommandHandler IdempotentCommandHandler { get; }

        protected SqsLambdaHandler(ITicketing ticketing, IIdempotentCommandHandler idempotentCommandHandler)
        {
            _ticketing = ticketing;
            IdempotentCommandHandler = idempotentCommandHandler;
        }

        protected abstract Task<string> InnerHandle(TSqsLambdaRequest request, CancellationToken cancellationToken);

        protected abstract TicketError? MapDomainException(DomainException exception);

        public async Task<Unit> Handle(TSqsLambdaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _ticketing.Pending(request.TicketId, cancellationToken);

                var etag = await InnerHandle(request, cancellationToken);

                await _ticketing.Complete(
                    request.TicketId,
                    new TicketResult(new ETagResponse(etag)),
                    cancellationToken);
            }
            catch (DomainException exception)
            {
                var ticketError = exception switch
                {
                    StreetNameIsNotFoundException => new TicketError(
                        ValidationErrorMessages.StreetName.StreetNameNotFound,
                        ValidationErrorCodes.StreetName.StreetNameNotFound),
                    StreetNameIsRemovedException => new TicketError(
                        ValidationErrorMessages.StreetName.StreetNameIsRemoved,
                        ValidationErrorCodes.StreetName.StreetNameIsRemoved),
                    _ => MapDomainException(exception)
                };

                ticketError ??= new TicketError(exception.Message, "");

                await _ticketing.Error(
                    request.TicketId,
                    ticketError,
                    cancellationToken);
            }

            return Unit.Value;
        }

        protected async Task<string> GetStreetNameHash(
            IMunicipalities municipalityRepository,
            MunicipalityId municipalityId,
            PersistentLocalId persistentLocalId,
            CancellationToken cancellationToken)
        {
            var municipality =
                await municipalityRepository.GetAsync(new MunicipalityStreamId(municipalityId), cancellationToken);
            var streetNameHash = municipality.GetStreetNameHash(persistentLocalId);
            return streetNameHash;
        }
    }
}
