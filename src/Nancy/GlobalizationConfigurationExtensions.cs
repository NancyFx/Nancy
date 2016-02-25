namespace Nancy
{
    using System.Collections.Generic;
    using Nancy.Configuration;

    /// <summary>
    /// Contains <see cref="GlobalizationConfiguration"/> configuration extensions for <see cref="INancyEnvironment"/>.
    /// </summary>
    public static class GlobalizationConfigurationExtensions
    {
        /// <summary>
        /// Configures <see cref="GlobalizationConfiguration"/>
        /// </summary>
        /// <param name="environment">An <see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="supportedCultureNames">Cultures that the application can accept</param>
        /// <param name="defaultCulture">Used to set a default culture for the application</param>
        /// <remarks>If defaultCulture not specified the first supported culture is used</remarks>
        public static void Cultures(this INancyEnvironment environment, IEnumerable<string> supportedCultureNames, string defaultCulture = null)
        {
            environment.AddValue(new GlobalizationConfiguration(
                supportedCultureNames: supportedCultureNames,
                defaultCulture: defaultCulture));
        }
    }
}