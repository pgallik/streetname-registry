namespace StreetNameRegistry.Municipality
{
    using System.Collections.Generic;
    using System.Linq;

    public class MunicipalityStreetNames : List<MunicipalityStreetName>
    {
        public bool HasPersistentLocalId(PersistentLocalId persistentLocalId)
            => this.Any(x => x.PersistentLocalId == persistentLocalId);

        public bool HasPersistentLocalId(PersistentLocalId persistentLocalId, out MunicipalityStreetName? streetName)
        {
            streetName = this.SingleOrDefault(x => x.PersistentLocalId == persistentLocalId);

            return streetName is not null;
        }

        public bool HasActiveStreetNameName(StreetNameName streetNameName)
            => this.Any(x => !x.IsRemoved
                && !x.IsRetired
                && !x.IsRejected
                && x.Names.HasMatch(streetNameName.Language, streetNameName.Name));

        public MunicipalityStreetName? FindByPersistentLocalId(PersistentLocalId persistentLocalId)
            => this.SingleOrDefault(x => x.PersistentLocalId == persistentLocalId);

        public MunicipalityStreetName GetByPersistentLocalId(PersistentLocalId persistentLocalId)
            => this.Single(x => x.PersistentLocalId == persistentLocalId);
    }
}
