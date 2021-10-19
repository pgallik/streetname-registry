namespace StreetNameRegistry.StreetName
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;

    public class StreetNameProvenanceFactory : IProvenanceFactory<Municipality>
    {
        public bool CanCreateFrom<TCommand>() => typeof(IHasProvenance).IsAssignableFrom(typeof(TCommand));
        public Provenance CreateFrom(object provenanceHolder, Municipality aggregate)
        {
            if (!(provenanceHolder is IHasProvenance provenance))
                throw new ApplicationException($"Cannot create provenance from {provenanceHolder.GetType().Name}");

            return new Provenance(
                provenance.Provenance.Timestamp,
                provenance.Provenance.Application,
                new Reason(provenance.Provenance.Reason),
                new Operator(provenance.Provenance.Operator),
                provenance.Provenance.Modification,
                provenance.Provenance.Organisation);
        }
    }
}
