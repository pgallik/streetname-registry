namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using Abstractions.Requests;

    public sealed class SqsStreetNameApproveRequest : SqsRequest, IHasBackOfficeRequest<StreetNameBackOfficeApproveRequest>
    {
        public StreetNameBackOfficeApproveRequest Request { get; set; }
    }
}
