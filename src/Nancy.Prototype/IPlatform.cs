namespace Nancy.Prototype
{
    using Nancy.Prototype.Scanning;

    public interface IPlatform
    {
        IAssemblyCatalog AssemblyCatalog { get; }

        ITypeCatalog TypeCatalog { get; }

        IBootstrapperLocator BootstrapperLocator { get; }
    }
}
