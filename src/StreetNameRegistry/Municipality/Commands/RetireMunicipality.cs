namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class RetireMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("980e00d1-b57b-4aee-a651-1de8d857d784");

        public MunicipalityId MunicipalityId { get; }
        public RetirementDate RetirementDate { get; }
        public Provenance Provenance { get; }

        public RetireMunicipality(
            MunicipalityId municipalityId,
            RetirementDate retirementDate,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            RetirementDate = retirementDate;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"RetireMunicipality-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return RetirementDate;

            foreach (var field in Provenance.GetIdentityFields())
            {
                yield return field;
            }
        }
    }
}
