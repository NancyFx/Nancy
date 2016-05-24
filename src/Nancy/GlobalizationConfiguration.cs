namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Globalization configuration
    /// </summary>
    public class GlobalizationConfiguration
    {
        /// <summary>
        /// A default instance of the <see cref="GlobalizationConfiguration"/> class
        /// </summary>
        public static readonly GlobalizationConfiguration Default = new GlobalizationConfiguration
        {
            SupportedCultureNames = new[] { CultureInfo.CurrentCulture.Name },
            DefaultCulture = CultureInfo.CurrentCulture.Name,
            DateTimeStyles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal
        };

        private GlobalizationConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalizationConfiguration"/> class
        /// </summary>
        /// <param name="supportedCultureNames">An array of supported cultures</param>
        /// <param name="defaultCulture">The default culture of the application</param>
        /// <param name="dateTimeStyles">The <see cref="DateTimeStyles"/> that should be used for date parsing.</param>
        public GlobalizationConfiguration(IEnumerable<string> supportedCultureNames, string defaultCulture = null, DateTimeStyles? dateTimeStyles = null)
        {
            if (supportedCultureNames == null)
            {
                throw new ConfigurationException("Invalid Globalization configuration. You must support at least one culture");
            }

            supportedCultureNames = supportedCultureNames.Where(cultureName => !string.IsNullOrEmpty(cultureName)).ToArray();

            if (!supportedCultureNames.Any())
            {
                throw new ConfigurationException("Invalid Globalization configuration. You must support at least one culture");
            }

            if (string.IsNullOrEmpty(defaultCulture))
            {
                defaultCulture = supportedCultureNames.First();
            }

            if (!supportedCultureNames.Contains(defaultCulture, StringComparer.OrdinalIgnoreCase))
            {
                throw new ConfigurationException("Invalid Globalization configuration. " + defaultCulture + " does not exist in the supported culture names");
            }

            this.DateTimeStyles = dateTimeStyles ?? Default.DateTimeStyles;
            this.DefaultCulture = defaultCulture;
            this.SupportedCultureNames = supportedCultureNames;
        }

        /// <summary>
        /// The <see cref="DateTimeStyles"/> that should be used for date parsing.
        /// </summary>
        public DateTimeStyles DateTimeStyles { get; private set; }

        /// <summary>
        /// The default culture for the application
        /// </summary>
        public string DefaultCulture { get; private set; }

        /// <summary>
        /// A set of supported cultures
        /// </summary>
        public IEnumerable<string> SupportedCultureNames { get; private set; }
    }
}
