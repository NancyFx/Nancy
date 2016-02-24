namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Globalization configuration
    /// </summary>
    public class GlobalizationConfiguration
    {
        /// <summary>
        /// A default instance of the <see cref="GlobalizationConfiguration"/> class
        /// </summary>
        public static readonly GlobalizationConfiguration Default = new GlobalizationConfiguration(supportedCultureNames: new[] { "en-US" }, defaultCulture: "en-US");

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalizationConfiguration"/> class
        /// </summary>
        /// <param name="supportedCultureNames">An array of supported cultures</param>
        /// <param name="defaultCulture">The default culture of the application</param>
        public GlobalizationConfiguration(IEnumerable<string> supportedCultureNames, string defaultCulture = null)
        {
            if (supportedCultureNames != null && supportedCultureNames.Any() && defaultCulture == null)
            {
                defaultCulture = supportedCultureNames.FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(defaultCulture) && !supportedCultureNames.Contains(defaultCulture, StringComparer.OrdinalIgnoreCase))
            {
                throw new ConfigurationException("Invalid Globalization configuration. " + defaultCulture + " does not exist in the supported culture names");
            }

            this.SupportedCultureNames = supportedCultureNames;
            this.DefaultCulture = defaultCulture;
        }

        /// <summary>
        /// A set of supported cultures
        /// </summary>
        public IEnumerable<string> SupportedCultureNames { get; private set; }

        /// <summary>
        /// The default culture for the application
        /// </summary>
        public string DefaultCulture { get; private set; }
    }
}
