namespace StreetNameRegistry.Api.BackOffice.Abstractions.Requests
{
    public class SqsStreetNameRejectRequest : SqsRequest
    {
        public int PersistentLocalId { get; set; }
    }
}
