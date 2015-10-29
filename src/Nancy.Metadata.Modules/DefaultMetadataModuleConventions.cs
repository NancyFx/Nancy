namespace Nancy.Metadata.Modules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This is a wrapper around the type
    /// 'IEnumerable{Func{INancyModule, IEnumerable{IMetadataModule}, IMetadataModule}}' and its
    /// only purpose is to make Ninject happy which was throwing an exception
    /// when constructor injecting this type.
    /// </summary>
    public class DefaultMetadataModuleConventions : IEnumerable<Func<INancyModule, IEnumerable<IMetadataModule>, IMetadataModule>>
    {
        private readonly IEnumerable<Func<INancyModule, IEnumerable<IMetadataModule>, IMetadataModule>> conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMetadataModuleConventions"/> class.
        /// </summary>
        public DefaultMetadataModuleConventions()
        {
            this.conventions = this.ConfigureMetadataModuleConventions();
        }

        public IEnumerator<Func<INancyModule, IEnumerable<IMetadataModule>, IMetadataModule>> GetEnumerator()
        {
            return this.conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private static string ReplaceModuleWithMetadataModule(string moduleName)
        {
            var i = moduleName.LastIndexOf("Module");
            return moduleName.Substring(0, i) + "MetadataModule";
        }

        private IEnumerable<Func<INancyModule, IEnumerable<IMetadataModule>, IMetadataModule>> ConfigureMetadataModuleConventions()
        {
            return new List<Func<INancyModule, IEnumerable<IMetadataModule>, IMetadataModule>>
                {
                    // 0 Handles: ./BlahModule -> ./BlahMetadataModule
                    (module, metadataModules) =>
                        {
                            var moduleType = module.GetType();
                            var moduleName = moduleType.FullName;
                            var metadataModuleName = ReplaceModuleWithMetadataModule(moduleName);

                            return metadataModules.FirstOrDefault(m =>
                                    string.Compare(m.GetType().FullName, metadataModuleName, StringComparison.OrdinalIgnoreCase) == 0);
                        },

                    // 1 Handles: ./BlahModule -> ./Metadata/BlahMetadataModule
                    (module, metadataModules) =>
                        {
                            var moduleType = module.GetType();
                            var moduleName = moduleType.FullName;
                            var parts = moduleName.Split('.').ToList();
                            parts.Insert(parts.Count - 1, "Metadata");

                            var metadataModuleName = ReplaceModuleWithMetadataModule(string.Join(".", (IEnumerable<string>)parts));

                            return metadataModules.FirstOrDefault(m =>
                                    string.Compare(m.GetType().FullName, metadataModuleName, StringComparison.OrdinalIgnoreCase) == 0);
                        },

                    // 2 Handles: ./Modules/BlahModule -> ../Metadata/BlahMetadataModule
                    (module, metadataModules) =>
                        {
                            var moduleType = module.GetType();
                            var moduleName = moduleType.FullName;
                            var parts = moduleName.Split('.').ToList();
                            parts[parts.Count - 2] = "Metadata";

                            var metadataModuleName = ReplaceModuleWithMetadataModule(string.Join(".", (IEnumerable<string>)parts));

                            return metadataModules.FirstOrDefault(m =>
                                    string.Compare(m.GetType().FullName, metadataModuleName, StringComparison.OrdinalIgnoreCase) == 0);
                        }
                };
        }
    }
}
