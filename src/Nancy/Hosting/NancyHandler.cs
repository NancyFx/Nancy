namespace Nancy.Hosting
{
    using System.Web;
    using Nancy.Extensions;

    public class NancyHandler
    {        
        private readonly INancyEngine engine;

        public NancyHandler(INancyEngine engine)
        {
            this.engine = engine;
        }

        public void ProcessRequest(HttpContextBase context)
        {
            if(IsRequestForFavicon(context))
            {
                return;
            }

            var request = CreateNancyRequest(context);
            var response = engine.HandleRequest(request);

            SetNancyResponseToHttpResponse(context, response);
        }

        private static bool IsRequestForFavicon(HttpContextBase context)
        {
            return context.Request.Url.AbsolutePath.Contains("favicon.ico");
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
            SetHttpResponseHeaders(context, response);

            context.Response.ContentType = response.ContentType;
            context.Response.StatusCode = (int)response.StatusCode;
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
