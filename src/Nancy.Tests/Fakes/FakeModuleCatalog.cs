using System;

namespace Nancy.Tests.Fakes
{
    using System.Collections.Generic;
    using System.Linq;
    public class FakeModuleCatalog : INancyModuleCatalog
    {
        private Dictionary<string, NancyModule> _Modules;

        /// <summary>
        /// Initializes a new instance of the FakeModuleCatalog class.
        /// </summary>
        public FakeModuleCatalog()
        {
            _Modules = new Dictionary<String, NancyModule>() { { "1", new FakeNancyModuleWithBasePath() }, { "2", new FakeNancyModuleWithoutBasePath() } };
        }

        public IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            return _Modules.Values.AsEnumerable();
        }

        public NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            return _Modules[moduleKey];
        }
    }
}
