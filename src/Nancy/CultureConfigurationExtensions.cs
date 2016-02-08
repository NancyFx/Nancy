namespace Nancy
{
    using System.Collections.Generic;
    using Nancy.Configuration;

    /// <summary>
    /// Contains <see cref="CultureConfiguration"/> configuration extensions for <see cref="INancyEnvironment"/>.
    /// </summary>
    public static class CultureConfigurationExtensions
    {
        /// <summary>
        /// Configures <see cref="CultureConfiguration"/>
        /// </summary>
        /// <param name="environment">An <see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="cultures">Cultures that the application can accept</param>
        public static void Cultures(this INancyEnvironment environment, IEnumerable<string> cultures)
        {
            environment.AddValue(new CultureConfiguration(
                cultureNames:cultures));
        }
    }
}