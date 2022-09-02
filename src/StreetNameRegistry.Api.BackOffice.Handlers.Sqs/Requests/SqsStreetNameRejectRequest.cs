namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using Abstractions.Requests;

    public class SqsStreetNameRejectRequest : SqsRequest, IHasBackOfficeRequest<StreetNameBackOfficeRejectRequest>
    {
        public StreetNameBackOfficeRejectRequest Request { get; set; }
    }
}
