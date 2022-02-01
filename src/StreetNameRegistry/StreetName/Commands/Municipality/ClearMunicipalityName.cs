namespace StreetNameRegistry.StreetName.Commands.Municipality
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class ClearMunicipalityName : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("4cb97bc5-4e1c-4cfd-b790-efe8f4afd30d");

        public string MunicipalityId { get; }
        public string Language { get; }
        public Provenance Provenance { get; }

        public ClearMunicipalityName(string municipalityId, string language, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Language = language;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(ClearMunicipalityName)}-{ToString()}");
        
        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return Language;
            yield return Provenance;
        }
    }
}
