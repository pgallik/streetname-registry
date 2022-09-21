namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using Abstractions.Requests;

    public sealed class SqsStreetNameRetireRequest : SqsRequest, IHasBackOfficeRequest<StreetNameBackOfficeRetireRequest>
    {
        public StreetNameBackOfficeRetireRequest Request { get; set; }
    }
}
