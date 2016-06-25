namespace Nancy.Session
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Session implementation
    /// </summary>
    public class Session : ISession
    {
        private readonly IDictionary<string, object> dictionary;
        private bool hasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        public Session() : this(new Dictionary<string, object>(0)){}

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public Session(IDictionary<string, object> dictionary)
        {
            this.dictionary = dictionary;
        }

        /// <summary>
        /// Gets the number of items stored
        /// </summary>
        public int Count
        {
            get { return this.dictionary.Count; }
        }

        /// <summary>
        /// Deletes all items
        /// </summary>
        public void DeleteAll()
        {
            if (this.Count > 0)
            {
                this.MarkAsChanged();
            }

            this.dictionary.Clear();
        }

        /// <summary>
        /// Delete an item with the given key
        /// </summary>
        /// <param name="key">Key to delete</param>
        public void Delete(string key)
        {
            if (this.dictionary.Remove(key)) {
                this.MarkAsChanged(); }
        }

        /// <summary>
        /// Gets or sets values
        /// </summary>
        /// <param name="key">The key whos value to get or set</param>
        /// <returns>The value, or null or the key didn't exist</returns>
        public object this[string key]
        {
            get { return this.dictionary.ContainsKey(key) ? this.dictionary[key] : null; }
            set
            {
                var existingValue = this[key] ?? new object();

                if (existingValue.Equals(value))
                {
                    return;
                }

                this.dictionary[key] = value;
                this.MarkAsChanged();
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
            return this.dictionary.GetEnumerator();
        }

        private void MarkAsChanged()
        {
            this.hasChanged = true;
        }
    }
}