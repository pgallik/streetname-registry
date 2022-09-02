namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda
{
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Text;
    using Abstractions.Response;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities.HexByteConvertor;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Municipality;
    using Requests;
    using TicketingService.Abstractions;

    public abstract class SqsLambdaHandler<TSqsLambdaRequest> : IRequestHandler<TSqsLambdaRequest>
        where TSqsLambdaRequest : SqsLambdaRequest
    {
        private readonly ITicketing _ticketing;
        private readonly ICommandHandlerResolver _bus;

        protected SqsLambdaHandler(
            ITicketing ticketing,
            ICommandHandlerResolver bus)
        {
            _ticketing = ticketing;
            _bus = bus;
        }

        protected abstract Task<string> InnerHandle(TSqsLambdaRequest request, CancellationToken cancellationToken);

        public async Task<Unit> Handle(TSqsLambdaRequest request, CancellationToken cancellationToken)
        {
            await _ticketing.Pending(request.TicketId, cancellationToken);

            var etag = await InnerHandle(request, cancellationToken);

            await _ticketing.Complete(request.TicketId, new TicketResult(new ETagResponse(etag)), cancellationToken);

            return Unit.Value;
        }

        protected async Task<long> IdempotentCommandHandlerDispatch(
                 IdempotencyContext context,
                 Guid? commandId,
                 object command,
                 IDictionary<string, object> metadata,
                 CancellationToken cancellationToken)
        {
            if (!commandId.HasValue || command == null)
                throw new ApiException("Ongeldig verzoek id.", StatusCodes.Status400BadRequest);

            // First check if the command id already has been processed
            var possibleProcessedCommand = await context
                .ProcessedCommands
                .Where(x => x.CommandId == commandId)
                .ToDictionaryAsync(x => x.CommandContentHash, x => x, cancellationToken);

            var contentHash = SHA512
                .Create()
                .ComputeHash(Encoding.UTF8.GetBytes(command.ToString()))
                .ToHexString();

            // It is possible we have a GUID collision, check the SHA-512 hash as well to see if it is really the same one.
            // Do nothing if commandId with contenthash exists
            if (possibleProcessedCommand.Any() && possibleProcessedCommand.ContainsKey(contentHash))
                throw new Abstractions.Exceptions.IdempotencyException("Already processed");

            var processedCommand = new ProcessedCommand(commandId.Value, contentHash);
            try
            {
                // Store commandId in Command Store if it does not exist
                await context.ProcessedCommands.AddAsync(processedCommand, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                // Do work
                return await CommandHandlerResolverExtensions.Dispatch(
                    _bus,
                    commandId.Value,
                    command,
                    metadata,
                    cancellationToken);
            }
            catch
            {
                // On exception, remove commandId from Command Store
                context.ProcessedCommands.Remove(processedCommand);
                context.SaveChanges();
                throw;
            }
        }

        protected async Task<string> GetStreetNameHash(
            IMunicipalities municipalityRepository,
            MunicipalityId municipalityId,
            PersistentLocalId persistentLocalId,
            CancellationToken cancellationToken)
        {
            var muniAggregate =
                await municipalityRepository.GetAsync(new MunicipalityStreamId(municipalityId), cancellationToken);
            var streetNameHash = muniAggregate.GetStreetNameHash(persistentLocalId);
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

    [Serializable]
    public sealed class IdempotencyException : Exception
    {
        public IdempotencyException(string? message)
            : base(message)
        { }

        private IdempotencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
