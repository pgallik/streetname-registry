namespace StreetNameRegistry.Exceptions
{
    using System;

    public class PersistentLocalIdAssignmentException : StreetNameRegistryException
    {
        public PersistentLocalIdAssignmentException() { }

        public PersistentLocalIdAssignmentException(string message) : base(message) { }

        public PersistentLocalIdAssignmentException(string message, Exception inner) : base(message, inner) { }
    }
}
