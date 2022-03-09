namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class ChangeMunicipalityNisCode: IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("7a754298-762b-4612-a080-b5b10baf19dd");

        public MunicipalityId MunicipalityId { get; }
        public NisCode NisCode { get; }
        public Provenance Provenance { get; }

        public ChangeMunicipalityNisCode(
            MunicipalityId municipalityId,
            NisCode nisCode,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"ChangeMunicipalityNisCode-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return NisCode;

            foreach (var field in Provenance.GetIdentityFields())
            {
                yield return field;
            }
        }
    }
}
