namespace StreetNameRegistry.StreetName.Commands.Municipality
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class ImportMunicipality: IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("c9b33779-73d8-49cf-9db7-d737bdbf3087");
        public MunicipalityId MunicipalityId { get; }
        public NisCode NisCode { get; }
        public Provenance Provenance { get; }

        public ImportMunicipality(
            MunicipalityId municipalityId,
            NisCode nisCode,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"ImportMunicipality-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return NisCode;
            yield return Provenance;
        }
    }
}
