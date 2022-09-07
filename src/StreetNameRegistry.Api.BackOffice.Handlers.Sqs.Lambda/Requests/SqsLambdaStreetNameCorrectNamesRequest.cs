namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions.Convertors;
    using Abstractions.Requests;
    using Municipality;
    using Municipality.Commands;

    public class SqsLambdaStreetNameCorrectNamesRequest : SqsLambdaRequest, IHasBackOfficeRequest<StreetNameBackOfficeCorrectNamesRequest>
    {
        public StreetNameBackOfficeCorrectNamesRequest Request { get; set; }

        /// <summary>
        /// Map to CorrectStreetNameNames command
        /// </summary>
        /// <returns>CorrectStreetNameNames.</returns>
        public CorrectStreetNameNames ToCommand(MunicipalityId municipalityId)
        {
            var names = new Names(
                Request.Straatnamen.Select(x => new StreetNameName(x.Value, TaalMapper.ToLanguage(x.Key))));

            return new CorrectStreetNameNames(
                municipalityId,
                new PersistentLocalId(Request.PersistentLocalId),
                names,
                Provenance);
        }
    }
}
