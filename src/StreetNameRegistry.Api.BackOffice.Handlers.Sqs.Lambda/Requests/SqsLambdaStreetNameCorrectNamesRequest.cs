namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions.Requests;

    public class SqsLambdaStreetNameCorrectNamesRequest : SqsLambdaRequest, IHasBackOfficeRequest<StreetNameBackOfficeCorrectNamesRequest>
    {
        public StreetNameBackOfficeCorrectNamesRequest Request { get; set; }
    }
}
