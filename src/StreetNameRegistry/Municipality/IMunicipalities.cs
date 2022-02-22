namespace StreetNameRegistry.Municipality
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public interface IMunicipalities : IAsyncRepository<Municipality, MunicipalityStreamId> { }

    public class MunicipalityStreamId : ValueObject<MunicipalityStreamId>
    {
        private readonly MunicipalityId _municipalityId;

        public MunicipalityStreamId(MunicipalityId municipalityId)
        {
            _municipalityId = municipalityId;
        }

        protected override IEnumerable<object> Reflect()
        {
            yield return _municipalityId;
        }

        public override string ToString() => $"municipality-{_municipalityId}";
    }
}
