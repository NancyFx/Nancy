namespace Nancy.ViewEngines.Razor
{
    using System.Collections.Generic;

	public interface IRazorConfiguration
	{
		bool AutoIncludeModelNamespace { get; }
	    
        IEnumerable<string> GetAssemblyNames();

	    IEnumerable<string> GetDefaultNamespaces();
	}
}
