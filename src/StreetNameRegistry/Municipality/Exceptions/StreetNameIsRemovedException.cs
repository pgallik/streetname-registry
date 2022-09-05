using System.Runtime.Serialization;

namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;

    [Serializable]
    public sealed class StreetNameIsRemovedException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameIsRemovedException()
        {
            PersistentLocalId = new PersistentLocalId(-1);
        }

        public StreetNameIsRemovedException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }

        private StreetNameIsRemovedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            PersistentLocalId = new PersistentLocalId(-1);
        }
    }
}
