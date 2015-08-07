namespace Nancy.Configuration
{
    using System;

    /// <summary>
    /// Default implementation of the <see cref="INancyEnvironmentConfigurator"/> interface.
    /// </summary>
    public class DefaultNancyEnvironmentConfigurator : INancyEnvironmentConfigurator
    {
        private readonly INancyEnvironmentFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNancyEnvironmentConfigurator"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="INancyEnvironmentFactory"/> instance to use when configuring an environment.</param>
        public DefaultNancyEnvironmentConfigurator(INancyEnvironmentFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Configures an <see cref="INancyEnvironment"/> instance.
        /// </summary>
        /// <param name="configuration">The configuration to apply to the environment.</param>
        /// <returns>An <see cref="INancyEnvironment"/> instance.</returns>
        public INancyEnvironment ConfigureEnvironment(Action<INancyEnvironment> configuration)
        {
            var environment = this.factory.CreateEnvironment();
            configuration.Invoke(environment);
            return environment;
        }
    }
}