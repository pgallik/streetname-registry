namespace StreetNameRegistry.Api.BackOffice.Handlers.Lambda.Requests
{
    using Abstractions;
    using Abstractions.Requests;
    using Municipality;
    using Municipality.Commands;

    public sealed class SqsLambdaStreetNameCorrectRetirementRequest :
        SqsLambdaRequest,
        IHasBackOfficeRequest<StreetNameBackOfficeCorrectRetirementRequest>,
        IHasStreetNamePersistentLocalId
    {
        public StreetNameBackOfficeCorrectRetirementRequest Request { get; set; }

        public int StreetNamePersistentLocalId => Request.PersistentLocalId;

        /// <summary>
        /// Map to CorrectStreetNameRetirement command
        /// </summary>
        /// <returns>CorrectStreetNameRetirement.</returns>
        public CorrectStreetNameRetirement ToCommand()
        {
            return new CorrectStreetNameRetirement(MunicipalityId, new PersistentLocalId(StreetNamePersistentLocalId), Provenance);
        }
    }
}
