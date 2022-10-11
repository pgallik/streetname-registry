namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using Abstractions.Requests;

    public sealed class SqsStreetNameCorrectRetirementRequest : SqsRequest, IHasBackOfficeRequest<StreetNameBackOfficeCorrectRetirementRequest>
    {
        public StreetNameBackOfficeCorrectRetirementRequest Request { get; set; }
    }
}
