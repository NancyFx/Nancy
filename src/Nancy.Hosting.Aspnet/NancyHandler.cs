using System.Linq;

namespace Nancy.Hosting.Aspnet
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
            var request = CreateNancyRequest(context);
            var response = engine.HandleRequest(request);

            SetNancyResponseToHttpResponse(context, response);
        }

        private static Request CreateNancyRequest(HttpContextBase context)
        {
            return new Request(
                (context.Request.Form["_method"] ?? context.Request.HttpMethod).ToUpperInvariant(),
                context.Request.AppRelativeCurrentExecutionFilePath.Replace("~", string.Empty),
                context.Request.Headers.ToDictionary(),
                context.Request.InputStream,
                context.Request.Url.Scheme,
                context.Request.Url.Query,
                context.Request.Files.Convert,
                context.Request.Form.ToDynamicDictionary);
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
            foreach (var header in response.Headers)
            {
                context.Response.AddHeader(header.Key, header.Value);
            }

            foreach(var cookie in response.Cookies)
            {
                context.Response.AddHeader("Set-Cookie", cookie.ToString());
            }
        }
    }
}