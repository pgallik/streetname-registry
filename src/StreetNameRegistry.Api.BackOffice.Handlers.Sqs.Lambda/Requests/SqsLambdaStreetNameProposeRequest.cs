namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions.Requests;

    public class SqsLambdaStreetNameProposeRequest : SqsLambdaRequest, IHasBackOfficeRequest<StreetNameBackOfficeProposeRequest>
    {
        public StreetNameBackOfficeProposeRequest Request { get; set; }
    }
}
