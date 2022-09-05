namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda
{
    using Abstractions;
    using Abstractions.Exceptions;
    using Abstractions.Response;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Handlers;
    using MediatR;
    using Municipality;
    using Municipality.Exceptions;
    using Requests;
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

        protected abstract TicketError? HandleDomainException(DomainException exception);

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
            catch (AggregateNotFoundException)
            {
                await _ticketing.Error(
                    request.TicketId,
                    new TicketError("", ""),
                    cancellationToken);
            }
            catch (IdempotencyException)
            {
                // Complete ticket with eTagResponse?
                // Deduplication should handle this...
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
                    _ => HandleDomainException(exception)
                };

                ticketError ??= new TicketError(exception.Message, "");

                await _ticketing.Error(
                    request.TicketId,
                    ticketError,
                    cancellationToken);
            }

            // Other exceptions are not caught, and thus will be retried.

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

        protected Provenance CreateFakeProvenance()
        {
            return new Provenance(
                NodaTime.SystemClock.Instance.GetCurrentInstant(),
                Application.BuildingRegistry,
                new Reason(""), // TODO: TBD
                new Operator(""), // TODO: from claims
                Modification.Insert,
                Organisation.DigitaalVlaanderen // TODO: from claims
            );
        }
    }
}
