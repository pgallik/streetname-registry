namespace StreetNameRegistry.Projections.Legacy.Interfaces
{
    using NodaTime;

    public interface IStreetNameListItem
    {
        string? NameDutch { get; set; }
        string? NameFrench { get; set; }
        string? NameGerman { get; set; }
        string? NameEnglish { get; set; }
        string? NameDutchSearch { get; set; }
        string? NameFrenchSearch { get; set; }
        string? NameGermanSearch { get; set; }
        string? NameEnglishSearch { get; set; }
        string? HomonymAdditionDutch { get; set; }
        string? HomonymAdditionFrench { get; set; }
        string? HomonymAdditionGerman { get; set; }
        string? HomonymAdditionEnglish { get; set; }
        StreetNameStatus? Status { get; set; }
        bool Removed { get; set; }
        Language? PrimaryLanguage { get; set; }
        Instant VersionTimestamp { get; set; }
    }
}
