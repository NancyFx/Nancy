namespace Nancy.Metadata.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Default implementation on how metadata modules are resolved by Nancy.
    /// </summary>
    public class DefaultMetadataModuleResolver : IMetadataModuleResolver
    {
        private readonly DefaultMetadataModuleConventions conventions;

        private readonly IEnumerable<IMetadataModule> metadataModules;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMetadataModuleResolver"/> class.
        /// </summary>
        /// <param name="conventions">The conventions that the resolver should use to determine which metadata module to return.</param>
        /// <param name="metadataModules">The metadata modules to use resolve against.</param>
        public DefaultMetadataModuleResolver(DefaultMetadataModuleConventions conventions, IEnumerable<IMetadataModule> metadataModules)
        {
            if (conventions == null)
            {
                throw new InvalidOperationException("Cannot create an instance of DefaultMetadataModuleResolver with conventions parameter having null value.");
            }

            if (metadataModules == null)
            {
                throw new InvalidOperationException("Cannot create an instance of DefaultMetadataModuleResolver with metadataModules parameter having null value.");
            }

            this.conventions = conventions;
            this.metadataModules = metadataModules;
        }

        /// <summary>
        /// Resolves a metadata module instance based on the provided information.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/>.</param>
        /// <returns>An <see cref="IMetadataModule"/> instance if one could be found, otherwise <see langword="null"/>.</returns>
        public IMetadataModule GetMetadataModule(INancyModule module)
        {
            return this.conventions
                .Select(convention => this.SafeInvokeConvention(convention, module))
                .FirstOrDefault(metadataModule => metadataModule != null);
        }

        private IMetadataModule SafeInvokeConvention(Func<INancyModule, IEnumerable<IMetadataModule>, IMetadataModule> convention, INancyModule module)
        {
            try
            {
                return convention.Invoke(module, this.metadataModules);
            }
            catch
            {
                return null;
            }
        }
    }
}