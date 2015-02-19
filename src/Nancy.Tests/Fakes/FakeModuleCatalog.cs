namespace Nancy.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FakeModuleCatalog : INancyModuleCatalog
    {
        private Dictionary<Type, INancyModule> _Modules;

        /// <summary>
        /// Initializes a new instance of the FakeModuleCatalog class.
        /// </summary>
        public FakeModuleCatalog()
        {
            _Modules = new Dictionary<Type, INancyModule>() { { typeof(FakeNancyModuleWithBasePath), new FakeNancyModuleWithBasePath() }, { typeof(FakeNancyModuleWithoutBasePath), new FakeNancyModuleWithoutBasePath() } };
        }

        public IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            return _Modules.Values.AsEnumerable();
        }

        public INancyModule GetModule(Type moduleType, NancyContext context)
        {
            return _Modules[moduleType];
        }
    }
}
