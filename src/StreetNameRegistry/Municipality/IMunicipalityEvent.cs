namespace StreetNameRegistry.Municipality
{
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;

    public interface IMunicipalityEvent : IHasMunicipalityId, IHasProvenance, ISetProvenance, IHaveHash
    { }
}
