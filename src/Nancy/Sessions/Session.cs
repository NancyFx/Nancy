namespace Nancy.Sessions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Session : ISession
    {
        private readonly IDictionary<string, object> dictionary;

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
            throw new NotImplementedException();
        }

        public void Delete(string key)
        {
            throw new NotImplementedException();
        }

        public object this[string key]
        {
            get { return dictionary.ContainsKey(key) ? dictionary[key] : null; }
            set { dictionary[key] = value; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
    }
}