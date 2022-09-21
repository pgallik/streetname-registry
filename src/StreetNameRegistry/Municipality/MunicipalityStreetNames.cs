namespace StreetNameRegistry.Municipality
{
    using System.Collections.Generic;
    using System.Linq;

    public sealed class MunicipalityStreetNames : List<MunicipalityStreetName>
    {
        public bool HasPersistentLocalId(PersistentLocalId persistentLocalId)
            => this.Any(x => x.PersistentLocalId == persistentLocalId);

        public bool HasActiveStreetNameName(StreetNameName streetNameName, PersistentLocalId existingStreetNamePersistentLocalId)
            => this.Any(x => !x.IsRemoved
                             && !x.IsRetired
                             && !x.IsRejected
                             && x.Names.HasMatch(streetNameName.Language, streetNameName.Name)
                             && x.PersistentLocalId != existingStreetNamePersistentLocalId);

        public MunicipalityStreetName? FindByPersistentLocalId(PersistentLocalId persistentLocalId)
            => this.SingleOrDefault(x => x.PersistentLocalId == persistentLocalId);

        public MunicipalityStreetName GetByPersistentLocalId(PersistentLocalId persistentLocalId)
            => this.Single(x => x.PersistentLocalId == persistentLocalId);
    }
}
