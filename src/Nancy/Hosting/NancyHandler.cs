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
            if (!string.IsNullOrEmpty(response.File))
            {
                context.Response.WriteFile(response.File);
            }
            else
            {
                response.Contents.Invoke(context.Response.OutputStream);    
            }            
        }

        private static void SetHttpResponseHeaders(HttpContextBase context, Response response)
        {
            foreach (var kvp in response.Headers)
            {
                context.Response.AddHeader(kvp.Key, kvp.Value);
            }
            foreach(var cookie in response.Cookies)
            {
                context.Response.AddHeader("Set-Cookie", cookie.ToString());
            }
        }
    }
}
