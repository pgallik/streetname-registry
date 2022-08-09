namespace StreetNameRegistry.Api.BackOffice.Abstractions.Requests
{
    public class SqsStreetNameApproveRequest : SqsRequest
    {
        public int PersistentLocalId { get; set; }
    }
}
