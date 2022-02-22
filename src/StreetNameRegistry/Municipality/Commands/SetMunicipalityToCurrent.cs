namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class SetMunicipalityToCurrent: IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("cd43a1d9-b843-499c-8e36-59aea02a1d42");

        public MunicipalityId MunicipalityId { get; }
        public Provenance Provenance { get; }

        public SetMunicipalityToCurrent(
            MunicipalityId municipalityId,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"SetMunicipalityToCurrent-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return Provenance;
        }
    }
}
