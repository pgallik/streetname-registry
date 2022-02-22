namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;

    // TODO: unused exception
    public class PersistentLocalIdAssignmentException : StreetNameRegistryException
    {
        public PersistentLocalIdAssignmentException() { }

        public PersistentLocalIdAssignmentException(string message) : base(message) { }

        public PersistentLocalIdAssignmentException(string message, Exception inner) : base(message, inner) { }
    }
}
