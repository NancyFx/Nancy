namespace Nancy.ViewEngines.Razor
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Configuration;

    /// <summary>
    /// 
    /// </summary>
	public class RazorConfiguration : IRazorConfiguration
    {
        private readonly RazorConfigurationSection razorConfigurationSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorConfiguration"/> class.
        /// </summary>
        public RazorConfiguration()
        {
            //this.razorConfigurationSection = ConfigurationManager.GetSection("razor") as RazorConfigurationSection;
        }

        public bool AutoIncludeModelNamespace 
		{
			get { return (this.razorConfigurationSection == null || (!this.razorConfigurationSection.DisableAutoIncludeModelNamespace)); }
		}

        public IEnumerable<string> GetAssemblyNames()
        {
            if (this.razorConfigurationSection == null || this.razorConfigurationSection.Assemblies == null)
            {
                return null;
            }
			
            return this.razorConfigurationSection.Assemblies.Select(a=>a.AssemblyName);
        }

        public IEnumerable<string> GetDefaultNamespaces()
		{
			if (this.razorConfigurationSection == null || this.razorConfigurationSection.Namespaces == null)
			{
			    return null;
			}

			return this.razorConfigurationSection.Namespaces.Select(n=>n.NamespaceName);
		}
	}
}