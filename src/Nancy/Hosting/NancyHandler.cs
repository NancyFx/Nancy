namespace Nancy.Hosting
{
    using System.Web;
    using Nancy.Extensions;
    using Sessions;

    public class NancyHandler
    {        
        private readonly INancyEngine engine;
        private readonly INancyApplication application;

        public NancyHandler(INancyEngine engine, INancyApplication application)
        {
            this.engine = engine;
            this.application = application;
        }

        public void ProcessRequest(HttpContextBase context)
        {
            if(IsRequestForFavicon(context))
            {
                return;
            }

            var sessionStore = application.SessionStore;
            var request = CreateNancyRequest(context, sessionStore);            
            var response = engine.HandleRequest(request);
            if (request.SessionWasLoaded)
            {
                sessionStore.Save(request.Session, response);
            }
            SetNancyResponseToHttpResponse(context, response);
        }

        private static bool IsRequestForFavicon(HttpContextBase context)
        {
            return context.Request.Url.AbsolutePath.Contains("favicon.ico");
        }

        private static IRequest CreateNancyRequest(HttpContextBase context, ISessionStore sessionStore)
        {
            return new Request(
                context.Request.HttpMethod,
                context.Request.Url.AbsolutePath,
                context.Request.Headers.ToDictionary(),
                context.Request.InputStream, 
                sessionStore);
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
