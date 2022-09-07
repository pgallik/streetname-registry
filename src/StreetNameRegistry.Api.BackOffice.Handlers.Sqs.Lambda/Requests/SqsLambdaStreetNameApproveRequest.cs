namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions;
    using Abstractions.Requests;
    using Municipality;
    using Municipality.Commands;

    public class SqsLambdaStreetNameApproveRequest :
        SqsLambdaRequest,
        IHasBackOfficeRequest<StreetNameBackOfficeApproveRequest>,
        IHasStreetNamePersistentLocalId
    {
        public StreetNameBackOfficeApproveRequest Request { get; set; }

        public int StreetNamePersistentLocalId => Request.PersistentLocalId;

        /// <summary>
        /// Map to ApproveStreetName command
        /// </summary>
        /// <returns>ApproveStreetName.</returns>
        public ApproveStreetName ToCommand(PersistentLocalId streetNamePersistentLocalId)
        {
            return new ApproveStreetName(MunicipalityId, streetNamePersistentLocalId, Provenance);
        }
    }
}
