namespace StreetNameRegistry.Api.BackOffice.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Requests;
    using Abstractions.Response;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using MediatR;
    using Municipality;
    using Municipality.Commands;

    public class StreetNameApproveHandler : BusHandler, IRequestHandler<StreetNameApproveRequest, ETagResponse>
    {
        private readonly BackOfficeContext _backOfficeContext;
        private readonly IMunicipalities _municipalities;
        private readonly IdempotencyContext _idempotencyContext;

        public StreetNameApproveHandler(
            ICommandHandlerResolver bus,
            BackOfficeContext backOfficeContext,
            IMunicipalities municipalities,
            IdempotencyContext idempotencyContext) : base(bus)
        {
            _backOfficeContext = backOfficeContext;
            _municipalities = municipalities;
            _idempotencyContext = idempotencyContext;
        }

        public async Task<ETagResponse> Handle(StreetNameApproveRequest request, CancellationToken cancellationToken)
        {
            var persistentLocalId = new PersistentLocalId(request.PersistentLocalId);

            var municipalityIdByPersistentLocalId = await _backOfficeContext
                .MunicipalityIdByPersistentLocalId
                .FindAsync(request.PersistentLocalId);

            var municipalityId = new MunicipalityId(municipalityIdByPersistentLocalId.MunicipalityId);

            var cmd = new ApproveStreetName(municipalityId, persistentLocalId, CreateFakeProvenance());
            await IdempotentCommandHandlerDispatch(_idempotencyContext, cmd.CreateCommandId(), cmd, request.Metadata, cancellationToken);

            var lastEventHash = await GetStreetNameHash(_municipalities, municipalityId, persistentLocalId, cancellationToken);
            return new ETagResponse(lastEventHash);
        }
    }
}
