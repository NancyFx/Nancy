namespace Nancy.Metadata.Modules
{
    using Nancy.Bootstrapper;

    /// <summary>
    /// Performs application registations for metadata modules.
    /// </summary>
    public class MetadataModuleRegistrations : Registrations
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MetadataModuleRegistrations"/> class, that performs
        /// the default registrations of the metadata modules types.
        /// </summary>
        /// <param name="typeCatalog">An <see cref="ITypeCatalog"/> instance.</param>
        public MetadataModuleRegistrations(ITypeCatalog typeCatalog) : base(typeCatalog)
        {
            this.Register<DefaultMetadataModuleConventions>();
            this.RegisterAll<IMetadataModule>();
            this.RegisterWithDefault<IMetadataModuleResolver>(typeof(DefaultMetadataModuleResolver));
        }
    }
}
