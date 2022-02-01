namespace StreetNameRegistry.StreetName.Commands.Municipality
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class NameMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("496f44dd-ae79-4e5f-93c0-c2b086143c09");

        public MunicipalityId MunicipalityId { get; }
        public MunicipalityName Name { get; }
        public Provenance Provenance { get; }

        public NameMunicipality(MunicipalityId municipalityId, MunicipalityName name, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Name = name;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(NameMunicipality)}-{ToString()}");
        
        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return Name;
            yield return Provenance;
        }
    }
}
