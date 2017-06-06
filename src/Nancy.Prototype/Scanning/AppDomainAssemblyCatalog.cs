#if NET452
namespace Nancy.Prototype.Scanning
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class AppDomainAssemblyCatalog : IAssemblyCatalog
    {
        public AppDomainAssemblyCatalog(AppDomain appDomain)
        {
            this.AppDomain = appDomain;
        }

        private AppDomain AppDomain { get; }

        public IEnumerable<Assembly> GetAssemblies()
        {
            return this.AppDomain.GetAssemblies();
        }
    }
}
#endif
