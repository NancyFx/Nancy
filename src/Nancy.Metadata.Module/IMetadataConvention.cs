namespace Nancy.Metadata.Module
{
    using System.Collections.Generic;

    public interface IMetadataConvention
    {
        bool CanUseModule(INancyModule module, IEnumerable<IMetadataModule> metadataModules);

        IMetadataModule DiscoverMetadataModule(INancyModule module, IEnumerable<IMetadataModule> metadataModules);
    }
}
