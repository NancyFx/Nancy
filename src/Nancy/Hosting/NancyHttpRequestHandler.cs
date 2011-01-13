namespace Nancy.Hosting
{
    using System.Web;
    using Routing;
    using System;
    using Nancy.BootStrapper;

    public sealed class BootStrapperEntry
    {
        public string Assembly { get; private set; }
        public string Name { get; private set; }

        public BootStrapperEntry(string assembly, string name)
        {
            Assembly = assembly;
            Name = name;
        }
    }

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
                bootStrapper = (INancyBootStrapper)Activator.CreateInstance(configBootStrapper.Assembly, configBootStrapper.Name);
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
            // TODO - Get a type fullname from the configuration file if one exists, similar to the httphandlers section
            return null;
        }
    }
}