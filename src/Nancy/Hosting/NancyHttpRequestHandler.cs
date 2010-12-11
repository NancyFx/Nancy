namespace Nancy.Hosting
{
    using System.Web;
    using Routing;

    public class NancyHttpRequestHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var engine = new NancyEngine(
                new AppDomainModuleLocator(),
                new RouteResolver());

            var wrappedContext = new HttpContextWrapper(context);
            var handler = new NancyHandler(engine);
            handler.ProcessRequest(wrappedContext);
        }
    }
}