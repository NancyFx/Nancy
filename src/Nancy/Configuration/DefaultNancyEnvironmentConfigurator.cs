namespace Nancy.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Default implementation of the <see cref="INancyEnvironmentConfigurator"/> interface.
    /// </summary>
    public class DefaultNancyEnvironmentConfigurator : INancyEnvironmentConfigurator
    {
        private readonly INancyEnvironmentFactory factory;
        private readonly IEnumerable<INancyDefaultConfigurationProvider> defaultConfigurationProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNancyEnvironmentConfigurator"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="INancyEnvironmentFactory"/> instance to use when configuring an environment.</param>
        /// <param name="defaultConfigurationProviders"><see cref="INancyDefaultConfigurationProvider"/> instances that should be used during the configuration of the environment.</param>
        public DefaultNancyEnvironmentConfigurator(INancyEnvironmentFactory factory, IEnumerable<INancyDefaultConfigurationProvider> defaultConfigurationProviders)
        {
            this.factory = factory;
            this.defaultConfigurationProviders = defaultConfigurationProviders;
        }

        /// <summary>
        /// Configures an <see cref="INancyEnvironment"/> instance.
        /// </summary>
        /// <param name="configuration">The configuration to apply to the environment.</param>
        /// <returns>An <see cref="INancyEnvironment"/> instance.</returns>
        public INancyEnvironment ConfigureEnvironment(Action<INancyEnvironment> configuration)
        {
            var environment =
                this.factory.CreateEnvironment();

            configuration.Invoke(environment);

            foreach (var configurationProvider in this.defaultConfigurationProviders)
            {
                var defaultConfiguration =
                    SafeGetDefaultConfiguration(configurationProvider);

                if (defaultConfiguration == null)
                {
                    continue;
                }

                var configurationKey =
                    configurationProvider.Key;

                if (environment.ContainsKey(configurationKey))
                {
                    continue;
                }

                environment.AddValue(configurationKey, defaultConfiguration);
            }

            return environment;
        }

        private static object SafeGetDefaultConfiguration(INancyDefaultConfigurationProvider configurationProviders)
        {
            try
            {
                return configurationProviders.GetDefaultConfiguration();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}