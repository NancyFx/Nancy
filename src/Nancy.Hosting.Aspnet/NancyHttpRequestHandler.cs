namespace Nancy.Hosting.Aspnet
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web;

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
            var bootstrapperEntry = GetConfiguredBootstrapperEntry();
            if (bootstrapperEntry == null)
            {
                return null;
            }

            var assemblyQualifiedName = string.Concat(bootstrapperEntry.Name, ", ", bootstrapperEntry.Assembly);

            var bootstrapperType = Type.GetType(assemblyQualifiedName);
            if (bootstrapperType == null)
            {
                throw new BootstrapperException(string.Format("Could not locate bootstrapper of type '{0}'.", assemblyQualifiedName));
            }

            try
            {
                return Activator.CreateInstance(bootstrapperType) as INancyBootstrapper;
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Could not initialize bootstrapper of type '{0}'.", bootstrapperType.FullName);
                throw new BootstrapperException(errorMessage, ex);
            }
        }

        private static BootstrapperEntry GetConfiguredBootstrapperEntry()
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

            if (string.IsNullOrWhiteSpace(bootstrapperOverrideType))
            {
                throw new BootstrapperException("The 'type' attribute of the 'bootstrapper' element under the 'nancyFx' config section is empty. Please specify the full type name.");
            }

            if (string.IsNullOrWhiteSpace(bootstrapperOverrideAssembly))
            {
                throw new BootstrapperException("The 'assembly' attribute of the 'bootstrapper' element under the 'nancyFx' config section is empty. Please specify the full assembly name.");
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