namespace Nancy.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FakeModuleCatalog : INancyModuleCatalog
    {
        private Dictionary<Type, INancyModule> modules;

        /// <summary>
        /// Initializes a new instance of the FakeModuleCatalog class.
        /// </summary>
        public FakeModuleCatalog()
        {
            this.modules = new Dictionary<Type, INancyModule>() { { typeof(FakeNancyModuleWithBasePath), new FakeNancyModuleWithBasePath() }, { typeof(FakeNancyModuleWithoutBasePath), new FakeNancyModuleWithoutBasePath() } };
        }

        public IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            return this.modules.Values.AsEnumerable();
        }

        public INancyModule GetModule(Type moduleType, NancyContext context)
        {
            return this.modules[moduleType];
        }
    }
}
