namespace StreetNameRegistry.Api.BackOffice.Handlers.Lambda.Requests
{
    using Abstractions;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.Sqs.Lambda.Requests;
    using Municipality;
    using Municipality.Commands;

    public sealed class SqsLambdaStreetNameCorrectApprovalRequest :
        SqsLambdaRequest,
        IHasBackOfficeRequest<StreetNameBackOfficeCorrectApprovalRequest>,
        IHasStreetNamePersistentLocalId
    {
        public StreetNameBackOfficeCorrectApprovalRequest Request { get; set; }

        public int StreetNamePersistentLocalId => Request.PersistentLocalId;

        /// <summary>
        /// Map to CorrectStreetNameApproval command
        /// </summary>
        /// <returns>CorrectStreetNameApproval.</returns>
        public CorrectStreetNameApproval ToCommand()
        {
            return new CorrectStreetNameApproval(this.MunicipalityPersistentLocalId(), new PersistentLocalId(StreetNamePersistentLocalId), Provenance);
        }
    }
}
