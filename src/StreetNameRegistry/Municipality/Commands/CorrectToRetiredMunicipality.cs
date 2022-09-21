namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class CorrectToRetiredMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("d208bb3f-8737-4fd0-a6d3-ca46063d8579");

        public MunicipalityId MunicipalityId { get; }
        public RetirementDate RetirementDate { get; }
        public Provenance Provenance { get; }

        public CorrectToRetiredMunicipality(MunicipalityId municipalityId, RetirementDate retirementDate, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            RetirementDate = retirementDate;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(CorrectToRetiredMunicipality)}-{ToString()}");
        
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
