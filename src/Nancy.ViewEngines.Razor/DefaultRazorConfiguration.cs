namespace Nancy.ViewEngines.Razor
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
	public class DefaultRazorConfiguration : IRazorConfiguration
    {
        private readonly RazorConfigurationSection razorConfigurationSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRazorConfiguration"/> class.
        /// </summary>
        public DefaultRazorConfiguration()
        {
            this.razorConfigurationSection = ConfigurationManager.GetSection("razor") as RazorConfigurationSection;
        }

        /// <summary>
        /// Gets a value indicating whether to automatically include the model's namespace in the generated code.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the model's namespace should be automatically included in the generated code; otherwise, <c>false</c>.
        /// </value>
        public bool AutoIncludeModelNamespace
		{
			get { return (this.razorConfigurationSection == null || (!this.razorConfigurationSection.DisableAutoIncludeModelNamespace)); }
		}

        /// <summary>
        /// Gets the assembly names to include in the generated assembly.
        /// </summary>
        public IEnumerable<string> GetAssemblyNames()
        {
            if (this.razorConfigurationSection == null || this.razorConfigurationSection.Assemblies == null)
            {
                return Enumerable.Empty<string>();
            }

            return this.razorConfigurationSection.Assemblies.Select(a => a.AssemblyName);
        }

        /// <summary>
        /// Gets the default namespaces to be included in the generated code.
        /// </summary>
        public IEnumerable<string> GetDefaultNamespaces()
        {
            if (this.razorConfigurationSection == null || this.razorConfigurationSection.Namespaces == null)
            {
                return Enumerable.Empty<string>();
            }

            return this.razorConfigurationSection.Namespaces.Select(n=>n.NamespaceName);
        }
    }
}