namespace Nancy.Configuration
{
    using System;

    /// <summary>
    /// Contains extensions for the <see cref="INancyEnvironment"/> type.
    /// </summary>
    public static class INancyEnvironmentExtensions
    {
        /// <summary>
        /// Adds a value to the environment, using the full name of the type defined by <typeparamref name="T"/> as the key.
        /// </summary>
        /// <param name="environment">The <see cref="INancyEnvironment"/> instance.</param>
        /// <param name="value">The value to store in the environment.</param>
        /// <typeparam name="T">The <see cref="Type"/> of the value to store in the environment.</typeparam>
        public static void AddValue<T>(this INancyEnvironment environment, T value)
        {
            environment.AddValue(typeof(T).FullName, value);
        }

        /// <summary>
        /// Gets a value from the environment, using the full name of the type defined by <typeparamref name="T"/> as the key.
        /// </summary>
        /// <param name="environment">The <see cref="INancyEnvironment"/> instance.</param>
        /// <typeparam name="T">The <see cref="Type"/> of the value to retreive from the environment.</typeparam>
        /// <returns></returns>
        public static T GetValue<T>(this INancyEnvironment environment)
        {
            return environment.GetValue<T>(typeof(T).FullName);
        }

        /// <summary>
        /// Gets a value from the environment, using the provided <paramref name="key"/>.
        /// </summary>
        /// <param name="environment">The <see cref="INancyEnvironment"/> instance.</param>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <typeparam name="T">The <see cref="Type"/> of the value to retreive from the environment.</typeparam>
        /// <returns>The stored value.</returns>
        public static T GetValue<T>(this INancyEnvironment environment, string key)
        {
            return (T) environment[key];
        }

        /// <summary>
        /// Gets a value from the environment, using the full name of the type defined by <typeparamref name="T"/> as the key. If
        /// the value could not be found, then a provided default value is returned.
        /// </summary>
        /// <param name="environment">The <see cref="INancyEnvironment"/> instance.</param>
        /// <param name="defaultValue">The value to return if no stored value could be found.</param>
        /// <typeparam name="T">The <see cref="Type"/> of the value to retreive from the environment.</typeparam>
        /// <returns>The stored value.</returns>
        public static T GetValueWithDefault<T>(this INancyEnvironment environment, T defaultValue)
        {
            T value;
            return environment.TryGetValue(typeof(T).FullName, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets a value from the environment, using the provided <paramref name="key"/>. If the value could not be found, then
        /// a provided default value is returned.
        /// </summary>
        /// <param name="environment">The <see cref="INancyEnvironment"/> instance.</param>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <param name="defaultValue">The value to return if no stored value could be found.</param>
        /// <typeparam name="T">The <see cref="Type"/> of the value to retreive from the environment.</typeparam>
        /// <returns>The stored value.</returns>
        public static T GetValueWithDefault<T>(this INancyEnvironment environment, string key, T defaultValue)
        {
            T value;
            return environment.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}