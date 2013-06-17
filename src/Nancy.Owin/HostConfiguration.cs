namespace Nancy.Owin
{
    /// <summary>
    /// Nancy specific host configuration for OWIN hosting
    /// </summary>
    public class HostConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether to request a client certificate or not.
        /// Defaults to false.
        /// </summary>
        public bool EnableClientCertificates { get; set; }

        public HostConfiguration()
        {
            this.EnableClientCertificates = false;
        }
    }
}