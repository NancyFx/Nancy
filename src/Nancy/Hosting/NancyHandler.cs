namespace Nancy.Hosting
{
    using System.Web;
    using Nancy.Extensions;
    using Nancy.Routing;

    public class NancyHandler
    {
        public void ProcessRequest(HttpContextBase context)
        {
            var url = context.Request.Url.AbsolutePath;
            if (url.Contains("favicon.ico"))
            {
                return;
            }

            var request = CreateNancyRequest(context);

            var engine = new NancyEngine(
                new AppDomainModuleLocator(), 
                new RouteResolver());

            var response = engine.HandleRequest(request);

            SetNancyResponseToHttpResponse(context, response);
        }

        private static IRequest CreateNancyRequest(HttpContextBase context)
        {
            return new Request(
                context.Request.HttpMethod,
                context.Request.Url.AbsolutePath,
                context.Request.Headers.ToDictionary(),
                context.Request.InputStream);
        }

        private static void SetNancyResponseToHttpResponse(HttpContextBase context, Response response)
        {
            context.Response.ContentType = response.ContentType;
            context.Response.StatusCode = (int)response.StatusCode;

            SetHttpResponseHeaders(context, response);

            response.Contents.Invoke(context.Response.OutputStream);
        }

        private static void SetHttpResponseHeaders(HttpContextBase context, Response response)
        {
            foreach (var key in response.Headers.Keys)
            {
                foreach (var value in response.Headers[key])
                {
                    context.Response.AddHeader(key, value);
                }
            }
        }
    }
}