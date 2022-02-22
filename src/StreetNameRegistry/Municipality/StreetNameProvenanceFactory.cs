namespace StreetNameRegistry.Municipality
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;

    public class StreetNameProvenanceFactory : IProvenanceFactory<Municipality>
    {
        public bool CanCreateFrom<TCommand>() => typeof(IHasProvenance).IsAssignableFrom(typeof(TCommand));
        public Provenance CreateFrom(object provenanceHolder, Municipality aggregate)
        {
            if (provenanceHolder is not IHasCommandProvenance provenance)
                throw new ApplicationException($"Cannot create provenance from {provenanceHolder.GetType().Name}");

            return provenance.Provenance;
        }
    }
}
