namespace StreetNameRegistry.StreetName.Events
{
    using System;

    public interface IHasMunicipalityId
    {
        Guid MunicipalityId { get; }
    }
}
