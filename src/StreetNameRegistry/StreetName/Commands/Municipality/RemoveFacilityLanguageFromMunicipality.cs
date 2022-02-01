namespace StreetNameRegistry.StreetName.Commands.Municipality
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class RemoveFacilityLanguageFromMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("8bb32674-a6bf-4b01-82ac-5a966952c821");

        public MunicipalityId MunicipalityId { get; }
        public Language Language { get; }
        public Provenance Provenance { get; }

        public RemoveFacilityLanguageFromMunicipality(MunicipalityId municipalityId, Language language, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Language = language;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(RemoveFacilityLanguageFromMunicipality)}-{ToString()}");
        
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
