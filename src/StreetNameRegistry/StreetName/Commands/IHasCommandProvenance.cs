namespace StreetNameRegistry.StreetName.Commands
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;

    //TODO: move to library
    public interface IHasCommandProvenance
    {
        public Provenance Provenance { get; }
        Guid CreateCommandId();
    }
}
