namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests
{
    using Abstractions.Requests;

    public class SqsLambdaStreetNameRetireRequest : SqsLambdaRequest, IHasBackOfficeRequest<StreetNameBackOfficeRetireRequest>
    {
        public StreetNameBackOfficeRetireRequest Request { get; set; }
    }
}
