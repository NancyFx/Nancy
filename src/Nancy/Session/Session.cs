namespace Nancy.Session
{
    using System.Collections;
    using System.Collections.Generic;

    public class Session : ISession
    {
        private readonly IDictionary<string, object> dictionary;
        private bool hasChanged;

        public Session() : this(new Dictionary<string, object>(0)){}
        public Session(IDictionary<string, object> dictionary)
        {
            this.dictionary = dictionary;
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public void DeleteAll()
        {
            if (Count > 0) { MarkAsChanged(); }
            dictionary.Clear();            
        }

        public void Delete(string key)
        {
            if (dictionary.Remove(key)) { MarkAsChanged(); }            
        }

        public object this[string key]
        {
            get { return dictionary.ContainsKey(key) ? dictionary[key] : null; }
            set
            {
                dictionary[key] = value;
                MarkAsChanged();
            }
        }

        public bool HasChanged
        {
            get { return this.hasChanged; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        private void MarkAsChanged()
        {
            hasChanged = true;
        }
    }
}