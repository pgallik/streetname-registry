namespace StreetNameRegistry.StreetName
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Events;

    public partial class Municipality : AggregateRootEntity
    {
        public static readonly Func<Municipality> Factory = () => new Municipality();

        public static Municipality Register(MunicipalityId municipalityId, NisCode nisCode)
        {
            var municipality = Factory();
            municipality.ApplyChange(new MunicipalityWasImported(municipalityId, nisCode));
            return municipality;
        }

        public void ProposeStreetName(Names streetNameNames, PersistentLocalId persistentLocalId)
        {
            ApplyChange(new StreetNameWasProposedV2(_municipalityId, _nisCode, streetNameNames, persistentLocalId));
        }

        public void ChangeNisCode(NisCode nisCode)
        {
            if (string.IsNullOrWhiteSpace(nisCode))
                throw new NoNisCodeException("NisCode of a municipality cannot be empty.");

            if(nisCode != _nisCode)
                ApplyChange(new MunicipalityNisCodeWasChanged(_municipalityId, nisCode));
        }

        public void BecomeCurrent()
        {
            if(MunicipalityStatus  != MunicipalityStatus.Current )
                ApplyChange(new MunicipalityBecameCurrent(_municipalityId));
        }

        public void Retire()
        {
            if(MunicipalityStatus  != MunicipalityStatus.Retired )
                ApplyChange(new MunicipalityWasRetired(_municipalityId));
        }

    }
}
