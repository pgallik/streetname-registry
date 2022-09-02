namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using Abstractions.Requests;

    public class SqsStreetNameApproveRequest : SqsRequest, IHasBackOfficeRequest<StreetNameBackOfficeApproveRequest>
    {
        public StreetNameBackOfficeApproveRequest Request { get; set; }
    }
}
