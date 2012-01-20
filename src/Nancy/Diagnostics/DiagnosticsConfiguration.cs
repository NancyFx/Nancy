namespace Nancy.Diagnostics
{
    /// <summary>
    /// Settings for the diagnostics dashboard
    /// </summary>
    public class DiagnosticsConfiguration
    {
        /// <summary>
        /// The password for accessing the diagnostics screen.
        /// This shoudl be secure :-)
        /// </summary>
        public string Password { get; set; }     

        /// <summary>
        /// Gets a value indicating whether the configuration is valid
        /// </summary>
        public bool Valid
        {
            get { return !string.IsNullOrWhiteSpace(this.Password); }
        }
    }
}