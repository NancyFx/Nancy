namespace Nancy.Hosting
{
    using System.Web;
    using Routing;

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
            _Engine = BootStrapper.NancyBootStrapperLocator.BootStrapper.GetEngine();
        }

        public void ProcessRequest(HttpContext context)
        {
            var wrappedContext = new HttpContextWrapper(context);
            var handler = new NancyHandler(_Engine);
            handler.ProcessRequest(wrappedContext);
        }        
    }
}