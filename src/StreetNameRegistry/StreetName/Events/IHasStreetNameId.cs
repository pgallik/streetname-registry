namespace StreetNameRegistry.StreetName.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.EventHandling;

    public interface IHasStreetNameId : IMessage
    {
        Guid StreetNameId { get; }
    }
}
