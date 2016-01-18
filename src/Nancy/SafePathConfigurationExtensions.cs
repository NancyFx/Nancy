namespace Nancy
{
    using System.Collections.Generic;
    using Nancy.Configuration;

    /// <summary>
    /// Contains <see cref="SafePathConfiguration"/> configuration extensions for <see cref="INancyEnvironment"/>.  
    /// </summary>
    public static class SafePathConfigurationExtensions
    {
        /// <summary>
        /// Configures <see cref="SafePathConfiguration"/> 
        /// </summary>
        /// <param name="environment"><see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="paths">Safe paths.</param>
        public static void AddSafePaths(this INancyEnvironment environment, params string[] paths)
        {
            environment.AddValue(new SafePathConfiguration(paths));
        }
    }
}

