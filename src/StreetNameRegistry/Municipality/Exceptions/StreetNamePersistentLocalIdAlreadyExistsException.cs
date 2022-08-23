namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class StreetNamePersistentLocalIdAlreadyExistsException : StreetNameRegistryException
    {
        public StreetNamePersistentLocalIdAlreadyExistsException()
        { }

        private StreetNamePersistentLocalIdAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
