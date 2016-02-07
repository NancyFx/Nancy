namespace Nancy.Demo.Razor.Localization
{
    using System;
    using Nancy.Bootstrapper;

    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(x => x.ResourceAssemblyProvider = typeof(CustomResourceAssemblyProvider));
            }
        }
    }
}
