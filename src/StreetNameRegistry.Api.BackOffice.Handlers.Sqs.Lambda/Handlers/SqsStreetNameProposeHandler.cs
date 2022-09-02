namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Lambda;
    using Municipality;
    using Requests;
    using TicketingService.Abstractions;

    public class SqsStreetNameProposeHandler : SqsLambdaHandler<SqsLambdaStreetNameProposeRequest>
    {
        private readonly IPersistentLocalIdGenerator _persistentLocalIdGenerator;
        private readonly IdempotencyContext _idempotencyContext;
        private readonly BackOfficeContext _backOfficeContext;
        private readonly IMunicipalities _municipalities;

        public SqsStreetNameProposeHandler(
            ITicketing ticketing,
            ICommandHandlerResolver bus,
            IPersistentLocalIdGenerator persistentLocalIdGenerator,
            IdempotencyContext idempotencyContext,
            BackOfficeContext backOfficeContext,
            IMunicipalities municipalities
            ) : base(ticketing, bus)
        {
            _persistentLocalIdGenerator = persistentLocalIdGenerator;
            _idempotencyContext = idempotencyContext;
            _backOfficeContext = backOfficeContext;
            _municipalities = municipalities;
        }

        protected override async Task<string> InnerHandle(SqsLambdaStreetNameProposeRequest request, CancellationToken cancellationToken)
        {
            var persistentLocalId = _persistentLocalIdGenerator.GenerateNextPersistentLocalId();
            var municipalityId = new MunicipalityId(new Guid(request.MessageGroupId));

            var cmd = request.Request.ToCommand(
                municipalityId,
                CreateFakeProvenance(),
                persistentLocalId);

            await IdempotentCommandHandlerDispatch(
                _idempotencyContext,
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
    }
}
