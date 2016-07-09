namespace Nancy
{
    using Configuration;

    /// <summary>
    /// Contains <see cref="ViewConfiguration"/> configuration extensions for <see cref="INancyEnvironment"/>.
    /// </summary>
    public static class ViewConfigurationExtensions
    {
        /// <summary>
        /// Configures <see cref="ViewConfiguration"/>.
        /// </summary>
        /// <param name="environment">An <see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="runtimeViewDiscovery"><see langword="true"/> if views can be discovered during runtime, otherwise <see langword="false"/>.</param>
        /// <param name="runtimeViewUpdates"><see langword="true"/> if views can be updated during runtime, otherwise <see langword="false"/>.</param>
        public static void Views(this INancyEnvironment environment, bool? runtimeViewDiscovery = false, bool? runtimeViewUpdates = false)
        {
            environment.AddValue(new ViewConfiguration(
                runtimeViewDiscovery: runtimeViewDiscovery ?? ViewConfiguration.Default.RuntimeViewDiscovery,
                runtimeViewUpdates: runtimeViewUpdates ?? ViewConfiguration.Default.RuntimeViewUpdates));
        }
    }
}
