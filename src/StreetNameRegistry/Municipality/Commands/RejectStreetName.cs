namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class RejectStreetName : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("f3e8a221-266d-48d4-8fb0-f42af7b9d3b0");

        public MunicipalityId MunicipalityId { get; }
        public PersistentLocalId PersistentLocalId { get; }
        public Provenance Provenance { get; }

        public RejectStreetName(
            MunicipalityId municipalityId,
            PersistentLocalId persistentLocalId,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Provenance = provenance;
            PersistentLocalId = persistentLocalId;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"RejectStreetName-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return PersistentLocalId;

            foreach (var field in Provenance.GetIdentityFields())
            {
                yield return field;
            }
        }
    }
}
