namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class StreetNameHasInvalidStatusException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameHasInvalidStatusException()
        {
            PersistentLocalId = new PersistentLocalId(-1);
        }

        public StreetNameHasInvalidStatusException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }

        private StreetNameHasInvalidStatusException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            PersistentLocalId = new PersistentLocalId(-1);
        }
    }
}
