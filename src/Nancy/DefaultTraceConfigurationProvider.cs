namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// Provides the default configuration for <see cref="TraceConfiguration"/>.
    /// </summary>
    public class DefaultTraceConfigurationProvider : NancyDefaultConfigurationProvider<TraceConfiguration>
    {
        private readonly IRuntimeEnvironmentInformation runtimeEnvironmentInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTraceConfigurationProvider"/> class.
        /// </summary>
        public DefaultTraceConfigurationProvider(IRuntimeEnvironmentInformation runtimeEnvironmentInformation)
        {
            this.runtimeEnvironmentInformation = runtimeEnvironmentInformation;
        }

        /// <summary>
        /// Gets the default configuration instance to register in the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <returns>The configuration instance</returns>
        public override TraceConfiguration GetDefaultConfiguration()
        {
            var isDebugMode =
                this.runtimeEnvironmentInformation.IsDebug;

            return new TraceConfiguration(
                enabled: false,
                displayErrorTraces: isDebugMode);
        }
    }
}
