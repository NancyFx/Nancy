namespace Nancy
{
    using System.Collections.Generic;

    public interface INancyModuleLocator
    {
        IEnumerable<NancyModule> GetModules();
    }
}