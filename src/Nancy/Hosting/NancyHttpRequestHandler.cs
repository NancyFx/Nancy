namespace Nancy.Hosting
{
    using System.Web;
    using Routing;
    using System;
    using Nancy.BootStrapper;
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
            INancyBootStrapper bootStrapper = null;

            var configBootStrapper = GetConfigBootStrapperType();

            if (configBootStrapper != null)
                bootStrapper = (INancyBootStrapper)(Activator.CreateInstance(configBootStrapper.Assembly, configBootStrapper.Name).Unwrap());
            else
                bootStrapper = NancyBootStrapperLocator.BootStrapper;

            _Engine = bootStrapper.GetEngine();
        }

        public void ProcessRequest(HttpContext context)
        {
            var wrappedContext = new HttpContextWrapper(context);
            var handler = new NancyHandler(_Engine);
            handler.ProcessRequest(wrappedContext);
        }

        private BootStrapperEntry GetConfigBootStrapperType()
        {
            var configurationSection = System.Configuration.ConfigurationManager.GetSection("nancyFx") as NancyFxSection;
            if (configurationSection == null)
                return null;

            var bootStrapperOverrideType = configurationSection.BootStrapper.Type;
            var bootStrapperOverrideAssembly = configurationSection.BootStrapper.Assembly;

            if (string.IsNullOrWhiteSpace(bootStrapperOverrideType) || string.IsNullOrWhiteSpace(bootStrapperOverrideAssembly))
                return null;

            return new BootStrapperEntry(bootStrapperOverrideAssembly, bootStrapperOverrideType);
        }
    }
}