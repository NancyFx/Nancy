namespace Nancy
{    
    using System.Collections.Generic;

    public interface INancyModuleLocator
    {
        IDictionary<string, IEnumerable<ModuleMeta>> GetModules();
    }
}