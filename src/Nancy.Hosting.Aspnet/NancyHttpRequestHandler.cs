namespace Nancy.Hosting.Aspnet
{
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web;
    using System;
    using Nancy.Bootstrapper;

    public class NancyHttpRequestHandler : IHttpAsyncHandler
    {
        private static INancyEngine engine;

        public bool IsReusable
        {
            get { return true; }
        }

        static NancyHttpRequestHandler()
        {
            var bootstrapper = GetBootstrapper();

            bootstrapper.Initialise();

            engine = bootstrapper.GetEngine();
        }

        private static INancyBootstrapper GetBootstrapper()
        {
            return GetConfigurationBootstrapper() ?? NancyBootstrapperLocator.Bootstrapper;
        }

        private static INancyBootstrapper GetConfigurationBootstrapper()
        {
            var configurationBootstrapperType = 
                GetConfigurationBootstrapperType();

            if (configurationBootstrapperType != null)
            {
                var bootstrapperType =
                    Type.GetType(string.Concat(configurationBootstrapperType.Name, ", ", configurationBootstrapperType.Assembly));

                return Activator.CreateInstance(bootstrapperType) as INancyBootstrapper;
            }

            return null;
        }

        private static BootstrapperEntry GetConfigurationBootstrapperType()
        {
            var configurationSection = 
                ConfigurationManager.GetSection("nancyFx") as NancyFxSection;

            if (configurationSection == null)
            {
                return null;
            }

            var bootstrapperOverrideType = 
                configurationSection.Bootstrapper.Type;

            var bootstrapperOverrideAssembly = 
                configurationSection.Bootstrapper.Assembly;

            if (string.IsNullOrWhiteSpace(bootstrapperOverrideType) || string.IsNullOrWhiteSpace(bootstrapperOverrideAssembly))
            {
                return null;
            }

            return new BootstrapperEntry(bootstrapperOverrideAssembly, bootstrapperOverrideType);
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotSupportedException();
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object state)
        {
            var wrappedContext = new HttpContextWrapper(context);
            var handler = new NancyHandler(engine);
            return handler.ProcessRequest(wrappedContext, cb, state);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            NancyHandler.EndProcessRequest((Task<Tuple<NancyContext, HttpContextBase>>)result);
        }
    }
}