using System;

namespace Nancy.Tests.Fakes
{
    public class FakeModuleKeyGenerator : BootStrapper.IModuleKeyGenerator
    {
        public int CallCount { get; set; }

        public string GetKeyForModuleType(Type moduleType)
        {
            CallCount++;

            if (moduleType == typeof(FakeNancyModuleWithBasePath))
                return "1";

            if (moduleType == typeof(FakeNancyModuleWithoutBasePath))
                return "2";

            return "none";
        }
    }
}
