namespace Nancy.Hosting.Aspnet
{
    using System.Configuration;
    using System.Web;
    using System;
    using Nancy.Bootstrapper;

    public class NancyHttpRequestHandler : IHttpHandler
    {
        private readonly INancyEngine engine;

        public static INancyBootstrapper bootstrapper;

        public NancyHttpRequestHandler()
        {
            var bootstrapper = 
                GetBootstrapper();

            bootstrapper.Initialise();

            this.engine = 
                bootstrapper.GetEngine();
        }

        private static INancyBootstrapper GetBootstrapper()
        {
            return GetConfigurationBootstrapper() ?? NancyBootstrapperLocator.Bootstrapper;
        }

        private static INancyBootstrapper GetConfigurationBootstrapper()
        {
            if (bootstrapper != null) return bootstrapper;

            var configurationBootstrapperType = 
                GetConfigurationBootstrapperType();

            if (configurationBootstrapperType != null)
            {
                var bootstrapperType =
                    Type.GetType(configurationBootstrapperType.Name);
                bootstrapper = (Activator.CreateInstance(bootstrapperType)) as INancyBootstrapper;
                return bootstrapper;
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
            var wrappedContext = new HttpContextWrapper(context);
            var handler = new NancyHandler(this.engine);
            handler.ProcessRequest(wrappedContext);
        }

        public bool IsReusable { get { return true; } }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            ProcessRequest(HttpContext.Current);
        }
        public void Dispose()
        {
        }
    }
}