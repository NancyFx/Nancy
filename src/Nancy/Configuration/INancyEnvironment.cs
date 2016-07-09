namespace Nancy.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality of a Nancy environment.
    /// </summary>
    public interface INancyEnvironment : IReadOnlyDictionary<string, object>, IHideObjectMembers
    {
        /// <summary>
        /// Adds a <paramref name="value"/>, using a provided <paramref name="key"/>, to the environment.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the value to add.</typeparam>
        /// <param name="key">The key to store the value as.</param>
        /// <param name="value">The value to store in the environment.</param>
        void AddValue<T>(string key, T value);

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the value to retrieve.</typeparam>
        /// <param name="key">The key to get the value for.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true" /> if the value could be retrieved, otherwise <see langword="false" />.</returns>
        bool TryGetValue<T>(string key, out T value);
    }
}