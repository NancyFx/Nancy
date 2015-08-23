namespace Nancy.Configuration
{
    using System;

    /// <summary>
    /// Defines the functionality for applying configuration to an <see cref="INancyEnvironment"/> instance.
    /// </summary>
    public interface INancyEnvironmentConfigurator : IHideObjectMembers
    {
        /// <summary>
        /// Configures an <see cref="INancyEnvironment"/> instance.
        /// </summary>
        /// <param name="configuration">The configuration to apply to the environment.</param>
        /// <returns>An <see cref="INancyEnvironment"/> instance.</returns>
        INancyEnvironment ConfigureEnvironment(Action<INancyEnvironment> configuration);
    }
}