namespace StreetNameRegistry.Api.BackOffice.Abstractions.Requests
{
    public class SqsStreetNameRetireRequest : SqsRequest
    {
        public int PersistentLocalId { get; set; }
    }
}
