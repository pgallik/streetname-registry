namespace StreetNameRegistry.StreetName
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    internal class StreetNameStatusLifetime : ValueObject<StreetNameStatusLifetime>, IComparable<StreetNameStatusLifetime>
    {
        public int? StatusId { get; set; }
        public StreetNameStatus? Status { get; set; }
        public DateTimeOffset? BeginDateTime { get; set; }

        public StreetNameStatusLifetime(StreetNameStatus status, int? statusId, DateTimeOffset? beginDateTime)
        {
            Status = status;
            StatusId = statusId;
            BeginDateTime = beginDateTime;
        }

        protected override IEnumerable<object> Reflect()
        {
            yield return StatusId;
            yield return Status;
            yield return BeginDateTime;
        }

        public int CompareTo(StreetNameStatusLifetime other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Nullable.Compare(BeginDateTime, other.BeginDateTime);
        }
    }
}
