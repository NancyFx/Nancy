namespace Nancy.ViewEngines
{
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Bootstrapper;

    public class ViewEngineStartup : IStartup
    {
        private readonly IEnumerable<IViewEngine> viewEngines;
        private readonly IViewLocationCache viewLocationCache;
        private readonly IViewCache viewCache;

        public ViewEngineStartup(IEnumerable<IViewEngine> viewEngines, IViewLocationCache viewLocationCache, IViewCache viewCache)
        {
            this.viewEngines = viewEngines;
            this.viewLocationCache = viewLocationCache;
            this.viewCache = viewCache;
        }

        /// <summary>
        /// Gets the type registrations to register for this startup task`
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the collection registrations to register for this startup task
        /// </summary>
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get { return null; }
        }

        public void Initialize(IPipelines pipelines)
        {
            foreach (var viewEngine in viewEngines)
            {
                viewEngine.Initialize(CreateViewEngineStartupContext(viewEngine));
            }
        }

        private ViewEngineStartupContext CreateViewEngineStartupContext(IViewEngine viewEngine)
        {
            return new ViewEngineStartupContext(
                this.viewCache,
                this.viewLocationCache,
                viewEngine.Extensions);
        }

        private IEnumerable<ViewLocationResult> GetViewsThatEngineCanRender(IViewEngine viewEngine)
        {
            return viewEngine.Extensions.SelectMany(extension => this.viewLocationCache.Where(x => x.Extension.Equals(extension))).ToList();
        }
    }
}