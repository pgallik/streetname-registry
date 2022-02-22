namespace StreetNameRegistry.Municipality
{
    using System;

    public readonly struct MunicipalityStatus
    {
        public static MunicipalityStatus Current = new MunicipalityStatus("Current");
        public static MunicipalityStatus Retired = new MunicipalityStatus("Retired");

        public string Status { get; }

        private MunicipalityStatus(string status) => Status = status;

        public static MunicipalityStatus? Parse(string status)
        {
            if (string.IsNullOrEmpty(status))
                return null;

            if (status != Retired.Status &&
                status != Current.Status)
                throw new NotImplementedException($"Cannot parse {status} to {nameof(MunicipalityStatus)}");

            return new MunicipalityStatus(status);
        }

        public static implicit operator string(MunicipalityStatus status) => status.Status;
    }
}
