namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using MediatR;
    using Abstractions;
    using Abstractions.Exceptions;
    using Abstractions.Response;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Requests;
    using Municipality;
    using Municipality.Exceptions;
    using Polly;
    using TicketingService.Abstractions;

    public abstract class SqsLambdaHandler<TSqsLambdaRequest> : IRequestHandler<TSqsLambdaRequest>
        where TSqsLambdaRequest : SqsLambdaRequest
    {
        private readonly ITicketing _ticketing;
        private readonly IMunicipalities _municipalities;

        protected IIdempotentCommandHandler IdempotentCommandHandler { get; }

        protected SqsLambdaHandler(
            IMunicipalities municipalities,
            ITicketing ticketing,
            IIdempotentCommandHandler idempotentCommandHandler)
        {
            _municipalities = municipalities;
            _ticketing = ticketing;
            IdempotentCommandHandler = idempotentCommandHandler;
        }

        protected abstract Task<string> InnerHandle(TSqsLambdaRequest request, CancellationToken cancellationToken);

        protected abstract TicketError? MapDomainException(DomainException exception);

        public async Task<Unit> Handle(TSqsLambdaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await ValidateIfMatchHeaderValue(request, cancellationToken);

                await _ticketing.Pending(request.TicketId, cancellationToken);

                var etag = string.Empty;
                await Retry(3, async () => etag = await InnerHandle(request, cancellationToken));

                await _ticketing.Complete(
                    request.TicketId,
                    new TicketResult(new ETagResponse(etag)),
                    cancellationToken);
            }
            catch (IfMatchHeaderValueMismatchException)
            {
                await _ticketing.Error(
                    request.TicketId,
                    new TicketError("Als de If-Match header niet overeenkomt met de laatste ETag.", "PreconditionFailed"),
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

        private async Task ValidateIfMatchHeaderValue(TSqsLambdaRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.IfMatchHeaderValue) || request is not IHasStreetNamePersistentLocalId id)
                return;

            var latestEventHash = await GetStreetNameHash(
                request.MunicipalityId,
                new PersistentLocalId(id.StreetNamePersistentLocalId),
                cancellationToken);

            var lastHashTag = new ETag(ETagType.Strong, latestEventHash);

            if (request.IfMatchHeaderValue != lastHashTag.ToString())
            {
                throw new IfMatchHeaderValueMismatchException();
            }
        }

        private async Task Retry(int numRetries, Func<Task> action)
        {
            var polly = Policy
                .Handle<Exception>()        
                .RetryAsync(numRetries);

            await polly.ExecuteAsync(async () => await action());
        }

        protected async Task<string> GetStreetNameHash(
            MunicipalityId municipalityId,
            PersistentLocalId persistentLocalId,
            CancellationToken cancellationToken)
        {
            var municipality = await _municipalities.GetAsync(new MunicipalityStreamId(municipalityId), cancellationToken);
            var streetNameHash = municipality.GetStreetNameHash(persistentLocalId);
            return streetNameHash;
        }
    }
}
