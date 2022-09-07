namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions.Convertors;
    using Abstractions.Requests;
    using Municipality;
    using Municipality.Commands;

    public class SqsLambdaStreetNameProposeRequest : SqsLambdaRequest, IHasBackOfficeRequest<StreetNameBackOfficeProposeRequest>
    {
        public StreetNameBackOfficeProposeRequest Request { get; set; }

        /// <summary>
        /// Map to ProposeStreetName command
        /// </summary>
        /// <returns>ProposeStreetName.</returns>
        public ProposeStreetName ToCommand(
            MunicipalityId municipalityId,
            PersistentLocalId persistentLocalId)
        {
            var names = new Names(
                Request.Straatnamen.Select(x => new StreetNameName(x.Value, TaalMapper.ToLanguage(x.Key))));

            return new ProposeStreetName(
                municipalityId,
                names,
                persistentLocalId,
                Provenance);
        }
    }
}
