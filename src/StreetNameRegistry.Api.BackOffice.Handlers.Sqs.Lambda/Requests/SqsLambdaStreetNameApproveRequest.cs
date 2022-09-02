namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions.Requests;

    public class SqsLambdaStreetNameApproveRequest : SqsLambdaRequest, IHasBackOfficeRequest<StreetNameBackOfficeApproveRequest>
    {
        public StreetNameBackOfficeApproveRequest Request { get; set; }
    }
}
