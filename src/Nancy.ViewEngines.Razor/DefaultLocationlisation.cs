
using System.Collections;
using System.Collections.Generic;

namespace Nancy.ViewEngines.Razor
{
    using System;
    using Nancy.Culture;
    using System.Globalization;

    public class DefaultLocationlisation : ILocationlisation
    {
        private readonly IDictionary<string, string> dictionary;

        public DefaultLocationlisation()
            : this(new Dictionary<string, string>(0))
        {
            this.dictionary.Add("Greeting-en-GB", "Hello sir");
            this.dictionary.Add("Greeting-de-DE", "Guten Tag");
            this.dictionary.Add("Greeting", "Howdy");
        }

        public DefaultLocationlisation(IDictionary<string, string> dictionary)
        {
            this.dictionary = dictionary;
        }

        public string this[string key]
        {
            get
            {
                if (dictionary.ContainsKey(key + "-" + CurrentCulture))
                {
                    return dictionary[key + "-" + CurrentCulture];
                }
                else
                {
                    return dictionary.ContainsKey(key) ? dictionary[key] : null;
                }
            }
            set { dictionary[key] = value; }
        }

        public CultureInfo CurrentCulture { get; set; }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}