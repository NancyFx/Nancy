namespace Nancy.Diagnostics
{
    using Cryptography;

    /// <summary>
    /// Settings for the diagnostics dashboard
    /// </summary>
    public class DiagnosticsConfiguration
    {
        /// <summary>
        /// Gets or sets password for accessing the diagnostics screen.
        /// This shoudl be secure :-)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the cryptography config to use for securing the diagnostics dashboard
        /// </summary>
        public CryptographyConfiguration CryptographyConfiguration { get; set; }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid
        /// </summary>
        public bool Valid
        {
            get { return !string.IsNullOrWhiteSpace(this.Password); }
        }

        public DiagnosticsConfiguration()
        {
            this.CryptographyConfiguration = CryptographyConfiguration.Default;
        }
    }
}