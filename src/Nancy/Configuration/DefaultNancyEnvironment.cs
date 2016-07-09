namespace Nancy.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Default implementation of the <see cref="INancyEnvironment"/> interface.
    /// </summary>
    public class DefaultNancyEnvironment : INancyEnvironment
    {
        private readonly IDictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Returns an enumerator that iterates through the environment.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey,TValue}.Enumerator"/> that can be used to iterate through the environment.</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the environment.
        /// </summary>
        /// <returns> An <see cref="IEnumerator"/> object that can be used to iterate through the environment.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets the number of elements in the environment.
        /// </summary>
        /// <returns>The number of elements in the environment.</returns>
        public int Count
        {
            get { return this.values.Count; }
        }

        /// <summary>
        /// Determines whether the environment contains an element that has the specified key.
        /// </summary>
        /// <returns><see langword="true"/> if the environment contains an element that has the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        /// <param name="key">The key to retrieve.</param>
        public bool ContainsKey(string key)
        {
            return this.values.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <returns><see langword="true"/> if the environment contains an element that has the specified key; otherwise, <see langword="false"/>.</returns>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        bool IReadOnlyDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return this.values.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the element that has the specified key in the environment.
        /// </summary>
        /// <returns>The element that has the specified key in the environment.</returns>
        /// <param name="key">The key to locate.</param>
        object IReadOnlyDictionary<string, object>.this[string key]
        {
            get { return this.values[key]; }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the environment.
        /// </summary>
        /// <returns>An enumerable collection that contains the keys in the environment.</returns>
        public IEnumerable<string> Keys
        {
            get { return this.values.Keys; }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the values in the environment.
        /// </summary>
        /// <returns>An enumerable collection that contains the values in the environment.</returns>
        public IEnumerable<object> Values
        {
            get { return this.values.Values; }
        }

        /// <summary>
        /// Adds a <paramref name="value"/>, using a provided <paramref name="key"/>, to the environment.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the value to add.</typeparam>
        /// <param name="key">The key to store the value as.</param>
        /// <param name="value">The value to store in the environment.</param>
        public void AddValue<T>(string key, T value)
        {
            this.values.Add(key, value);
        }

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the value to retrieve.</typeparam>
        /// <param name="key">The key to get the value for.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true" /> if the value could be retrieved, otherwise <see langword="false" />.</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            object objectValue;

            if (this.values.TryGetValue(key, out objectValue))
            {
                value = (T) objectValue;
                return true;
            }

            value = default(T);
            return false;
        }
    }
}