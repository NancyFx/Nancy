namespace Nancy.Configuration
{
    /// <summary>
    /// Default (abstract) implementation of <see cref="INancyDefaultConfigurationProvider" /> interface.
    /// </summary>
    /// <typeparam name="T">The type of the configuration object.</typeparam>
    public abstract class NancyDefaultConfigurationProvider<T> : INancyDefaultConfigurationProvider
    {
        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        public abstract T GetDefaultConfiguration();

        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        object INancyDefaultConfigurationProvider.GetDefaultConfiguration()
        {
            return this.GetDefaultConfiguration();
        }

        /// <summary>
        /// Gets the full type name of <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the key.</returns>
        public virtual string Key
        {
            get { return typeof(T).FullName; }
        }
    }
}