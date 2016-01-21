namespace Nancy
{
    using System.Collections.Generic;
    using Nancy.Configuration;

    /// <summary>
    /// Contains <see cref="StaticContentConfiguration"/> configuration extensions for <see cref="INancyEnvironment"/>.  
    /// </summary>
    public static class StaticContentConfigurationExtensions
    {
        /// <summary>
        /// Configures <see cref="StaticContentConfiguration"/> 
        /// </summary>
        /// <param name="environment"><see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="paths">Safe paths.</param>
        public static void StaticContent(this INancyEnvironment environment, params string[] paths)
        {
            environment.AddValue(new StaticContentConfiguration(paths));
        }
    }
}

