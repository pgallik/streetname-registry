namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions.Requests;

    public class SqsLambdaStreetNameRejectRequest : SqsLambdaRequest, IHasBackOfficeRequest<StreetNameBackOfficeRejectRequest>
    {
        public StreetNameBackOfficeRejectRequest Request { get; set; }
    }
}
