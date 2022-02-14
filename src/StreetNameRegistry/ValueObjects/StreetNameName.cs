namespace StreetNameRegistry
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public class StreetNameName : ValueObject<StreetNameName>
    {
        public string Name { get; }

        public Language? Language { get; }

        public StreetNameName(string name, Language? language)
        {
            Name = name;
            Language = language;
        }


        protected override IEnumerable<object> Reflect()
        {
            yield return Name;
            yield return Language;
        }

        public override string ToString() => $"{Name} ({Language})";
    }
}
