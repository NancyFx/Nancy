namespace Nancy.Hosting.Aspnet
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using Bootstrapper;
    using IO;
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

            using (var nancyContext = this.engine.HandleRequest(request))
            {
                SetNancyResponseToHttpResponse(context, nancyContext.Response);
            }
        }

        private static Request CreateNancyRequest(HttpContextBase context)
        {
            var expectedRequestLength =
                GetExpectedRequestLength(context.Request.Headers.ToDictionary());

            var nancyUrl = new Url
                               {
                                   Scheme = context.Request.Url.Scheme,
                                   HostName = context.Request.Url.Host,
                                   Port = context.Request.Url.Port,
                                   BasePath = context.Request.ApplicationPath.TrimEnd('/'),
                                   Path = context.Request.AppRelativeCurrentExecutionFilePath.Replace("~", string.Empty),
                                   Query = context.Request.Url.Query,
                                   Fragment = context.Request.Url.Fragment,
                               };

            return new Request(
                context.Request.HttpMethod.ToUpperInvariant(),
                nancyUrl,
                RequestStream.FromStream(context.Request.InputStream, expectedRequestLength, true),
                context.Request.Headers.ToDictionary(),
                context.Request.UserHostAddress);
        }

        private static long GetExpectedRequestLength(IDictionary<string, IEnumerable<string>> incomingHeaders)
        {
            if (incomingHeaders == null)
            {
                return 0;
            }

            if (!incomingHeaders.ContainsKey("Content-Length"))
            {
                return 0;
            }

            var headerValue =
                incomingHeaders["Content-Length"].SingleOrDefault();

            if (headerValue == null)
            {
                return 0;
            }

            long contentLength;
            if (!long.TryParse(headerValue, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength))
            {
                return 0;
            }

            return contentLength;
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