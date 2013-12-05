namespace Nancy.Embedded
{
    using Nancy.Bootstrapper;
    using Nancy.ViewEngines;

    /// <summary>
    /// Application registrations supporting views embedded in an assembly
    /// </summary>
    public class Registrations : ApplicationRegistrations
    {
        /// <summary>
        /// Registers the <see cref="EmbeddedViewLocationProvider"/> with the container to enable
        /// embedded views
        /// </summary>
        public Registrations()
        {
            this.Register<IResourceAssemblyProvider>(typeof(ResourceAssemblyProvider));
            this.Register<IResourceReader>(typeof(DefaultResourceReader));
            this.Register<IViewLocationProvider>(typeof(EmbeddedViewLocationProvider));
        }
    }
}
