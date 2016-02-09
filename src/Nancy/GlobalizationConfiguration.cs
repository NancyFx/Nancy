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
        /// <param name="supportedCultureNames"></param>
        public GlobalizationConfiguration(IEnumerable<string> supportedCultureNames)
        {
            this.SupportedCultureNames = supportedCultureNames;
        }

        /// <summary>
        /// A set of supported cultures
        /// </summary>
        public IEnumerable<string> SupportedCultureNames { get; private set; }
    }
}
