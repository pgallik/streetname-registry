namespace StreetNameRegistry.Api.BackOffice.Abstractions.Response;

public record PersistentLocalIdETagResponse(int PersistentLocalId, string LastEventHash);
