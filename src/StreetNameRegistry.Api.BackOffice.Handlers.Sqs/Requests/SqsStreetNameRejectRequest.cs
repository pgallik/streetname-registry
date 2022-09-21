namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using Abstractions.Requests;

    public sealed class SqsStreetNameRejectRequest : SqsRequest, IHasBackOfficeRequest<StreetNameBackOfficeRejectRequest>
    {
        public StreetNameBackOfficeRejectRequest Request { get; set; }
    }
}
