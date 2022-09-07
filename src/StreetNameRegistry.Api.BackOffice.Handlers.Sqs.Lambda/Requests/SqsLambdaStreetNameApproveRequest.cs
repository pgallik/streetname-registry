namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions.Requests;
    using Municipality;
    using Municipality.Commands;

    public class SqsLambdaStreetNameApproveRequest : SqsLambdaRequest, IHasBackOfficeRequest<StreetNameBackOfficeApproveRequest>
    {
        public StreetNameBackOfficeApproveRequest Request { get; set; }

        /// <summary>
        /// Map to ApproveStreetName command
        /// </summary>
        /// <returns>ApproveStreetName.</returns>
        public ApproveStreetName ToCommand(MunicipalityId municipalityId, PersistentLocalId streetNamePersistentLocalId)
        {
            return new ApproveStreetName(municipalityId, streetNamePersistentLocalId, Provenance);
        }
    }
}
