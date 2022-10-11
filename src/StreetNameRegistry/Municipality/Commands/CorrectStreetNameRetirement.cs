namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class CorrectStreetNameRetirement : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("959f6387-f5a5-41b1-90a4-f0414cc99c5e");

        public MunicipalityId MunicipalityId { get; }
        public PersistentLocalId PersistentLocalId { get; }
        public Provenance Provenance { get; }

        public CorrectStreetNameRetirement(
            MunicipalityId municipalityId,
            PersistentLocalId persistentLocalId,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            PersistentLocalId = persistentLocalId;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"CorrectStreetNameRetirement-{ToString()}");

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
