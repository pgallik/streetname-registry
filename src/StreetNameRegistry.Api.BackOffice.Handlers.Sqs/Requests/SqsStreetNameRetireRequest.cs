namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using Abstractions.Requests;

    public class SqsStreetNameRetireRequest : SqsRequest, IHasBackOfficeRequest<StreetNameBackOfficeRetireRequest>
    {
        public StreetNameBackOfficeRetireRequest Request { get; set; }
    }
}
