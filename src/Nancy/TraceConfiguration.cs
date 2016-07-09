namespace Nancy
{
    /// <summary>
    /// Configuration for tracing.
    /// </summary>
    public class TraceConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceConfiguration"/> class.
        /// </summary>
        /// <param name="enabled">Determines if tracing should be enabled.</param>
        /// <param name="displayErrorTraces">Determines if traces should be displayed in error messages.</param>
        public TraceConfiguration(bool enabled, bool displayErrorTraces)
        {
            this.Enabled = enabled;
            this.DisplayErrorTraces = displayErrorTraces;
        }

        /// <summary>
        /// Gets a value indicating whether or not to enable request tracing.
        /// </summary>
        /// <value><see langword="true"/> if tracing should be enabled, otherwise <see langword="false"/>.</value>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not to display traces in error messages.
        /// </summary>
        /// <value><see langword="true"/> traces should be displayed in error messages, otherwise <see langword="false"/>.</value>
        public bool DisplayErrorTraces { get; private set; }
    }
}
