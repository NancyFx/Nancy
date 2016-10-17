using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nancy.ViewEngines.Razor
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    ///
    /// </summary>
    public class DefaultRazorConfiguration : IRazorConfiguration
    {
        private readonly IConfigurationSection razorSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRazorConfiguration"/> class.
        /// </summary>
        public DefaultRazorConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables();
            var configuration = configurationBuilder.Build();
            this.razorSection = configuration.GetSection("Razor");
        }

        /// <summary>
        /// Gets the assembly names to include in the generated assembly.
        /// </summary>
        public IEnumerable<string> GetAssemblyNames()
        {
            return razorSection?.GetValue<string[]>("Assemblies") ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets the default namespaces to be included in the generated code.
        /// </summary>
        public IEnumerable<string> GetDefaultNamespaces()
        {
            return razorSection?.GetValue<string[]>("Namespaces") ?? Enumerable.Empty<string>();
        }
    }
}