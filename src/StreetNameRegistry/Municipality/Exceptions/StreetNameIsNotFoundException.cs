namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class StreetNameIsNotFoundException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameIsNotFoundException()
        {
            PersistentLocalId = new PersistentLocalId(-1);
        }

        public StreetNameIsNotFoundException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }

        private StreetNameIsNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            PersistentLocalId = new PersistentLocalId(-1);
        }
    }
}
