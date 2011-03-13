using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.ViewEngines.Razor
{
	public interface IRazorConfiguration
	{
		bool AutoIncludeModelNamespace { get; }
		IEnumerable<string> GetDefaultNamespaces();
		IEnumerable<string> GetAssemblyNames();
	}
}
