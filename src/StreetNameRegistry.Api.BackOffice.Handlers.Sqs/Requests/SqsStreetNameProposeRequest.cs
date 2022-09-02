namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests
{
    using Abstractions.Requests;

    public class SqsStreetNameProposeRequest : SqsRequest, IHasBackOfficeRequest<StreetNameBackOfficeProposeRequest>
    {
        public StreetNameBackOfficeProposeRequest Request { get; set; }
    }
}
