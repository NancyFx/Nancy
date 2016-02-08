namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default <see cref="CultureConfiguration"/>.
    /// </summary>
    public class DefaultCultureConfigurationProvider : NancyDefaultConfigurationProvider<CultureConfiguration>
    {
        /// <summary>
        /// Gets the default <see cref="CultureConfiguration"/> configuration instance to register in the <see cref="INancyEnvironment"/> .
        /// </summary>
        /// <returns>The configuration instance</returns>
        public override CultureConfiguration GetDefaultConfiguration()
        {
            return CultureConfiguration.Default;
        }
    }
}
