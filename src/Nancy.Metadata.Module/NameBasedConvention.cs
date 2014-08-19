namespace Nancy.Metadata.Module
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NameBasedConvention : IMetadataConvention
    {
        public bool CanUseModule(INancyModule module, IEnumerable<IMetadataModule> metadataModules)
        {
            return GetMetadataModule(module, metadataModules) != null;
        }
        public IMetadataModule DiscoverMetadataModule(INancyModule module, IEnumerable<IMetadataModule> metadataModules)
        {
            return GetMetadataModule(module, metadataModules);
        }

        private IMetadataModule GetMetadataModule(INancyModule module, IEnumerable<IMetadataModule> metadataModules)
        {
            try
            {
                var moduleType = module.GetType();
                var moduleName = moduleType.FullName;
                var metadataModuleName = ReplaceModuleWithMetadataModule(moduleName);

                return metadataModules.FirstOrDefault(m =>
                        string.Compare(m.GetType().FullName, metadataModuleName, StringComparison.OrdinalIgnoreCase) == 0);
            }
            catch
            {
                return null;
            }
        }

        private static string ReplaceModuleWithMetadataModule(string moduleName)
        {
            var i = moduleName.LastIndexOf("Module");
            return moduleName.Substring(0, i) + "MetadataModule";
        }
    }
}
