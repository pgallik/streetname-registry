namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions.Requests;
    using Municipality;
    using Municipality.Commands;

    public class SqsLambdaStreetNameRejectRequest : SqsLambdaRequest, IHasBackOfficeRequest<StreetNameBackOfficeRejectRequest>
    {
        public StreetNameBackOfficeRejectRequest Request { get; set; }

        /// <summary>
        /// Map to RejectStreetName command
        /// </summary>
        /// <returns>RejectStreetName.</returns>
        public RejectStreetName ToCommand(MunicipalityId municipalityId, PersistentLocalId streetNamePersistentLocalId)
        {
            return new RejectStreetName(municipalityId, streetNamePersistentLocalId, Provenance);
        }
    }
}
