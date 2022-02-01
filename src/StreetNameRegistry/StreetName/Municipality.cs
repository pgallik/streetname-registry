namespace StreetNameRegistry.StreetName
{
    using System;
    using System.Linq;
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

        public void DefineOrChangeNisCode(NisCode nisCode)
        {
            if (string.IsNullOrWhiteSpace(nisCode))
            {
                throw new NoNisCodeException("NisCode of a municipality cannot be empty.");
            }

            if (nisCode != _nisCode)
            {
                ApplyChange(new MunicipalityNisCodeWasChanged(_municipalityId, nisCode));
            }
        }

        public void BecomeCurrent()
        {
            if (MunicipalityStatus != MunicipalityStatus.Current)
            {
                ApplyChange(new MunicipalityBecameCurrent(_municipalityId));
            }
        }

        public void CorrectToCurrent()
        {
            if (MunicipalityStatus != MunicipalityStatus.Current)
            {
                ApplyChange(new MunicipalityWasCorrectedToCurrent(_municipalityId));
            }
        }

        public void CorrectToRetired()
        {
            if (MunicipalityStatus != MunicipalityStatus.Retired)
            {
                ApplyChange(new MunicipalityWasCorrectedToRetired(_municipalityId));
            }
        }

        public void Retire()
        {
            if (MunicipalityStatus != MunicipalityStatus.Retired)
            {
                ApplyChange(new MunicipalityWasRetired(_municipalityId));
            }
        }

        public void NameMunicipality(MunicipalityName name)
        {
            ApplyChange(new MunicipalityWasNamed(_municipalityId, name));
        }

        public void AddOfficialLanguage(Language language)
        {
            if (!_officialLanguages.Contains(language))
                ApplyChange(new MunicipalityOfficialLanguageWasAdded(_municipalityId, language));
        }

        public void RemoveOfficialLanguage(Language language)
        {
            if (_officialLanguages.Contains(language))
                ApplyChange(new MunicipalityOfficialLanguageWasRemoved(_municipalityId, language));
        }

        public void AddFacilityLanguage(Language language)
        {
            if (!_facilityLanguages.Contains(language))
                ApplyChange(new MunicipalityFacilityLanguageWasAdded(_municipalityId, language));
        }

        public void RemoveFacilityLanguage(Language language)
        {
            if (_facilityLanguages.Contains(language))
                ApplyChange(new MunicipalityFacilityLanguageWasRemoved(_municipalityId, language));
        }
    }
}
