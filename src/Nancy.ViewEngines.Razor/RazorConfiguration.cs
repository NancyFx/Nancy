using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Nancy.ViewEngines.Razor
{
	public class RazorConfiguration : IRazorConfiguration
	{
		private RazorConfigurationSection RazorConfigurationSection { get; set; }

		public bool AutoIncludeModelNamespace 
		{
			get
			{
				return (this.RazorConfigurationSection == null || (!this.RazorConfigurationSection.DisableAutoIncludeModelNamespace));
			}
		}

		public RazorConfiguration()
		{
			this.RazorConfigurationSection = ConfigurationManager.GetSection("razor") as RazorConfigurationSection;
		}

		public IEnumerable<string> GetDefaultNamespaces()
		{
			if (this.RazorConfigurationSection == null
				|| this.RazorConfigurationSection.Namespaces == null)
				return null;

			return this.RazorConfigurationSection.Namespaces.Select(n=>n.NamespaceName);
		}
		public IEnumerable<string> GetAssemblyNames()
		{
			if (this.RazorConfigurationSection == null
				|| this.RazorConfigurationSection.Assemblies == null)
				return null;
			
			return this.RazorConfigurationSection.Assemblies.Select(a=>a.AssemblyName);
		}

	}
}
