namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// Contains <see cref="TraceConfiguration"/> configuration extensions for <see cref="INancyEnvironment"/>.
    /// </summary>
    public static class TraceConfigurationExtensions
    {
        /// <summary>
        /// Configures <see cref="TraceConfiguration"/>.
        /// </summary>
        /// <param name="environment">An <see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="enabled"><see langword="true"/> if tracing should be enabled, otherwise <see langword="false"/>.</param>
        /// <param name="displayErrorTraces"><see langword="true"/> traces should be displayed in error messages, otherwise <see langword="false"/>.</param>
        public static void Tracing(this INancyEnvironment environment, bool enabled, bool displayErrorTraces)
        {
            environment.AddValue(new TraceConfiguration(
                enabled: enabled,
                displayErrorTraces: displayErrorTraces));
        }
    }
}
