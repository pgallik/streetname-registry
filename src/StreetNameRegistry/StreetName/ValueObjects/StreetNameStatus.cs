namespace StreetNameRegistry.StreetName
{
    using System;

    public enum StreetNameStatus
    {
        Proposed = 0,
        Current = 1,
        Retired = 2
    }

    public static class StreetNameStatusHelpers
    {
        public static Municipality.StreetNameStatus ToMunicipalityStreetNameStatus(this StreetNameStatus? status)
        {
            if (!status.HasValue)
                throw new InvalidOperationException("Cannot convert null StreetName.Status.");

            return status switch
            {
                StreetNameStatus.Current => Municipality.StreetNameStatus.Current,
                StreetNameStatus.Proposed => Municipality.StreetNameStatus.Proposed,
                StreetNameStatus.Retired => Municipality.StreetNameStatus.Retired,
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, $"Non existing status '{status}'.")
            };
        }
    }
}
