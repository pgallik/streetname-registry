namespace StreetNameRegistry.StreetName.Events
{
    using System;

    public interface IHasStreetNameId
    {
        Guid StreetNameId { get; }
    }
}
