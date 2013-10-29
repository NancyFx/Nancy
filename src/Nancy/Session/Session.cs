namespace Nancy.Session
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Session implementation
    /// </summary>
    public class Session : ISession
    {
        private readonly IDictionary<string, object> dictionary;
        private bool hasChanged;

        public Session() : this(new Dictionary<string, object>(0)){}
        public Session(IDictionary<string, object> dictionary)
        {
            this.dictionary = dictionary;
        }

        /// <summary>
        /// Gets the number of items stored
        /// </summary>
        public int Count
        {
            get { return dictionary.Count; }
        }

        /// <summary>
        /// Deletes all items
        /// </summary>
        public void DeleteAll()
        {
            if (Count > 0)
            {
                MarkAsChanged();
            }

            dictionary.Clear();            
        }

        /// <summary>
        /// Delete an item with the given key
        /// </summary>
        /// <param name="key">Key to delete</param>
        public void Delete(string key)
        {
            if (dictionary.Remove(key)) { MarkAsChanged(); }            
        }

        /// <summary>
        /// Gets or sets values
        /// </summary>
        /// <param name="key">The key whos value to get or set</param>
        /// <returns>The value, or null or the key didn't exist</returns>
        public object this[string key]
        {
            get { return dictionary.ContainsKey(key) ? dictionary[key] : null; }
            set
            {
                var existingValue = this[key] ?? new Object();

                if (existingValue.Equals(value))
                {
                    return;
                }

                dictionary[key] = value;
                MarkAsChanged();
            }
        }

        /// <summary>
        /// Gets whether the session has changed
        /// </summary>
        public bool HasChanged
        {
            get { return this.hasChanged; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
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