namespace Nancy.Hosting
{
    using System.Web;

    public class NancyHttpRequestHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var wrappedContext = new HttpContextWrapper(context);
            var handler = new NancyHandler();
            handler.ProcessRequest(wrappedContext);
        }
    }
}