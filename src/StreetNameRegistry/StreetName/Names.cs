namespace StreetNameRegistry.StreetName
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Names : List<StreetNameName>
    {
        public Names()
        { }

        public Names(IEnumerable<StreetNameName> streetNameNames)
            : base(streetNameNames)
        { }

        public Names(IDictionary<Language, string> streetNameNames)
            : base(streetNameNames.Select(x => new StreetNameName(x.Value, x.Key)))
        { }

        public bool HasMatch(Language? language, string name)
            => this.Any(x => x.Language == language && x.Name == name);

        public bool HasLanguage(Language? language)
            => this.Any(name => name.Language == language);

        public void AddOrUpdate(Language? language, string name)
        {
            if (HasLanguage(language))
                Update(language, name);
            else
                Add(language, name);
        }

        public IDictionary<Language, string> ToDictionary() =>
            this.ToDictionary(
                x => x.Language ?? throw new ArgumentNullException(nameof(StreetNameName.Language)),
                x => x.Name);
        
        private void Update(Language? language, string name)
        {
            if (!HasLanguage(language))
                throw new IndexOutOfRangeException();

            Remove(language);
            Add(language, name);
        }

        private void Add(Language? language, string name)
        {
            if (HasLanguage(language))
                throw new ApplicationException($"Already name present with language {language}");

            Add(new StreetNameName(name, language));
        }

        public void Remove(Language? language)
        {
            var index = GetIndexByLanguage(language);
            if (index != -1)
                RemoveAt(index);
        }

        private int GetIndexByLanguage(Language? language)
            => FindIndex(name => name.Language == language);
    }
}
