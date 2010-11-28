namespace Nancy.Hosting
{
    using System.Reflection;
    using System.Web;
    using Nancy.Routing;

    public class NancyHttpRequestHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var url = context.Request.Url.AbsolutePath;
            if (url.Contains("favicon.ico"))
            {
                return;
            }

            var request = CreateNancyRequest(context);

            var assembly = 
                context.ApplicationInstance.GetType().BaseType.Assembly;

            var engine =
                new NancyEngine(new NancyModuleLocator(assembly), new RouteResolver());

            var response = engine.HandleRequest(request);

            SetNancyResponseToHttpResponse(context, response);
        }

        private static IRequest CreateNancyRequest(HttpContext context)
        {
            return new Request(
                context.Request.RequestType,
                context.Request.Url.AbsolutePath);
        }

        private static void SetNancyResponseToHttpResponse(HttpContext context, Response response)
        {
            if (!string.IsNullOrEmpty(response.ContentType))
            {
                context.Response.ContentType = response.ContentType;
            }

            context.Response.StatusCode = (int)response.StatusCode;
            context.Response.Write(response.Contents);
        }
    }
}