namespace Nancy.Demo.SuperSimpleViewEngine.Localization
{
    using Bootstrapper;

    public class DemoBoostrapper : DefaultNancyBootstrapper
    {
        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(x => x.ResourceAssemblyProvider = typeof(CustomResourceAssemblyProvider));
            }
        }
    }
}