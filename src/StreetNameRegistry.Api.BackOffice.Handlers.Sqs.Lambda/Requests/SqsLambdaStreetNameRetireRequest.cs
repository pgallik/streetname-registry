namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions.Requests;
    using Municipality;
    using Municipality.Commands;

    public class SqsLambdaStreetNameRetireRequest : SqsLambdaRequest, IHasBackOfficeRequest<StreetNameBackOfficeRetireRequest>
    {
        public StreetNameBackOfficeRetireRequest Request { get; set; }

        /// <summary>
        /// Map to RetireStreetName command
        /// </summary>
        /// <returns>RetireStreetName.</returns>
        public RetireStreetName ToCommand(MunicipalityId municipalityId, PersistentLocalId streetNamePersistentLocalId)
        {
            return new RetireStreetName(municipalityId, streetNamePersistentLocalId, Provenance);
        }
    }
}
