namespace Nancy
{
    using System.Collections.Generic;

    /// <summary>
    /// Globalization configuration
    /// </summary>
    public class GlobalizationConfiguration
    {
        /// <summary>
        /// A default instance of the <see cref="GlobalizationConfiguration"/> class
        /// </summary>
        public static readonly GlobalizationConfiguration Default = new GlobalizationConfiguration(supportedCultureNames: new[] { "en-us" });

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalizationConfiguration"/> class
        /// </summary>
        /// <param name="supportedCultureNames">An array of supported cultures</param>
        /// <param name="defaultCulture">The default culture of the application</param>
        public GlobalizationConfiguration(IEnumerable<string> supportedCultureNames, string defaultCulture = null)
        {
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
        public string DefaultCulture { get; set; }
    }
}
