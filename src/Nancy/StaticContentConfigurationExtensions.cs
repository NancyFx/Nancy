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
        /// <param name="environment">An <see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="safepaths">Paths that the application consider safe to return static content from</param>
        public static void StaticContent(this INancyEnvironment environment, params string[] safepaths)
        {
            environment.AddValue(new StaticContentConfiguration(
               safePaths: safepaths));
        }
    }
}

