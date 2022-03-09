namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class ApproveStreetName : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("a2a4c55b-5e6d-42a9-b81e-0b69d9bd10c1");

        public MunicipalityId MunicipalityId { get; }
        public PersistentLocalId PersistentLocalId { get; }
        public Provenance Provenance { get; }

        public ApproveStreetName(
            MunicipalityId municipalityId,
            PersistentLocalId persistentLocalId,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Provenance = provenance;
            PersistentLocalId = persistentLocalId;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"ApproveStreetName-{ToString()}");

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
