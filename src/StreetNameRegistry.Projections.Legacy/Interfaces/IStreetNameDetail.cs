namespace StreetNameRegistry.Projections.Legacy.Interfaces
{
    using NodaTime;

    public interface IStreetNameDetail
    {
        string? NisCode { get; set; }
        string? NameDutch { get; set; }
        string? NameFrench { get; set; }
        string? NameGerman { get; set; }
        string? NameEnglish { get; set; }
        string? HomonymAdditionDutch { get; set; }
        string? HomonymAdditionFrench { get; set; }
        string? HomonymAdditionGerman { get; set; }
        string? HomonymAdditionEnglish { get; set; }
        StreetNameStatus? Status { get; set; }
        bool Removed { get; set; }
        Instant VersionTimestamp { get; set; }
    }
}
