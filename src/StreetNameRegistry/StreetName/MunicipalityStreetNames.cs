namespace StreetNameRegistry.StreetName
{
    using System.Collections.Generic;
    using System.Linq;

    internal class MunicipalityStreetNames : List<MunicipalityStreetName>
    {
        public bool HasPersistentLocalId(PersistentLocalId persistentLocalId)
            => this.Any(x => x.PersistentLocalId == persistentLocalId);

        public bool HasStreetNameName(StreetNameName streetNameName)
            => this.Any(x => x.Names.HasMatch(streetNameName.Language, streetNameName.Name));
    }
}
