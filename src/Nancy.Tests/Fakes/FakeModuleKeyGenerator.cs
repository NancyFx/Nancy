using System;

namespace Nancy.Tests.Fakes
{
    public class FakeModuleKeyGenerator : BootStrapper.IModuleKeyGenerator
    {
        public string GetKeyForModuleType(Type moduleType)
        {
            if (moduleType == typeof(FakeNancyModuleWithBasePath))
                return "1";

            if (moduleType == typeof(FakeNancyModuleWithoutBasePath))
                return "2";

            return "none";
        }
    }
}
