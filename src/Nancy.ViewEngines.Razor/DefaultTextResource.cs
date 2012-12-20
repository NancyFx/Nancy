namespace Nancy.ViewEngines.Razor
{
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;

    public class DefaultTextResource : ITextResource
    {
        private readonly IRenderContext renderContext;
        private readonly IDictionary<string, IDictionary<string, string>> dictionary;

        public DefaultTextResource()
        {
            this.dictionary = new Dictionary<string, IDictionary<string, string>>();
            this.dictionary.Add("Greeting", new Dictionary<string, string>() { { "en-GB", "Hello Sir" }, { "de-DE", "Guten Tag" }, { "en-US", "Howdy" } });
        }

        public IEnumerator<KeyValuePair<string, IDictionary<string,string>>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string this[string key, string culture]
        {
            get
            {
                if (!dictionary.ContainsKey(key))
                {
                    return null;

                }

                return dictionary[key].ContainsKey(culture) ? dictionary[key][culture] : null;
            }

            set { dictionary[key][culture] = value; }
        }

    }
}