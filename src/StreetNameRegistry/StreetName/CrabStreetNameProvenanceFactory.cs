namespace StreetNameRegistry.StreetName
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;

    public class CrabStreetNameProvenanceFactory : CrabProvenanceFactory, IProvenanceFactory<StreetName>
    {
        public bool CanCreateFrom<TCommand>() => typeof(IHasCrabProvenance).IsAssignableFrom(typeof(TCommand));

        public Provenance CreateFrom(object provenanceHolder, StreetName aggregate)
        {
            if (!(provenanceHolder is IHasCrabProvenance crabProvenance))
                throw new ApplicationException($"Cannot create provenance from {provenanceHolder.GetType().Name}");

            return CreateFrom(
                aggregate.LastModificationBasedOnCrab,
                crabProvenance.Timestamp,
                crabProvenance.Modification,
                crabProvenance.Operator,
                crabProvenance.Organisation);
        }
    }
}
