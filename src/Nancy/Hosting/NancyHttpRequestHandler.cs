namespace Nancy.Hosting
{
    using System.Web;
    using Routing;
    using System;
    using Nancy.Bootstrapper;
    using System.Configuration;

    public class NancyHttpRequestHandler : IHttpHandler
    {
        // TODO - make static?
        private readonly INancyEngine _Engine;

        public bool IsReusable
        {
            get { return true; }
        }

        public NancyHttpRequestHandler()
        {
            INancyBootstrapper bootstrapper = null;

            var configBootstrapper = GetConfigBootstrapperType();

            if (configBootstrapper != null)
                bootstrapper = (INancyBootstrapper)(Activator.CreateInstance(configBootstrapper.Assembly, configBootstrapper.Name).Unwrap());
            else
                bootstrapper = NancyBootstrapperLocator.Bootstrapper;

            _Engine = bootstrapper.GetEngine();
        }

        public void ProcessRequest(HttpContext context)
        {
            var wrappedContext = new HttpContextWrapper(context);
            var handler = new NancyHandler(_Engine);
            handler.ProcessRequest(wrappedContext);
        }

        private BootstrapperEntry GetConfigBootstrapperType()
        {
            var configurationSection = System.Configuration.ConfigurationManager.GetSection("nancyFx") as NancyFxSection;
            if (configurationSection == null)
                return null;

            var bootstrapperOverrideType = configurationSection.Bootstrapper.Type;
            var bootstrapperOverrideAssembly = configurationSection.Bootstrapper.Assembly;

            if (string.IsNullOrWhiteSpace(bootstrapperOverrideType) || string.IsNullOrWhiteSpace(bootstrapperOverrideAssembly))
                return null;

            return new BootstrapperEntry(bootstrapperOverrideAssembly, bootstrapperOverrideType);
        }
    }
}