namespace StreetNameRegistry.StreetName.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class RetireMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("980e00d1-b57b-4aee-a651-1de8d857d784");
        public MunicipalityId MunicipalityId { get; }
        public Provenance Provenance { get; }

        public RetireMunicipality(
            MunicipalityId municipalityId,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"RetireMunicipality-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return Provenance;
        }
    }
}
