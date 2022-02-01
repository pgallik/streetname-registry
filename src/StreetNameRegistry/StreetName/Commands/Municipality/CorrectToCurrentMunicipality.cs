namespace StreetNameRegistry.StreetName.Commands.Municipality
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class CorrectToCurrentMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("6d9cffe3-129d-4094-9eb4-f45f72e850f3");

        public MunicipalityId MunicipalityId { get; }
        public Provenance Provenance { get; }

        public CorrectToCurrentMunicipality(MunicipalityId municipalityId, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(CorrectToCurrentMunicipality)}-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return Provenance;
        }
    }
}
