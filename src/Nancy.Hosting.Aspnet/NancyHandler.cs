namespace Nancy.Hosting.Aspnet
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using IO;
    using Nancy.Extensions;

    /// <summary>
    /// Bridges the communication between Nancy and ASP.NET based hosting.
    /// </summary>
    public class NancyHandler
    {        
        private readonly INancyEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyHandler"/> type for the specified <paramref name="engine"/>.
        /// </summary>
        /// <param name="engine">An <see cref="INancyEngine"/> instance, that should be used by the handler.</param>
        public NancyHandler(INancyEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// Processes the ASP.NET request with Nancy.
        /// </summary>
        /// <param name="context">The <see cref="HttpContextBase"/> of the request.</param>
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

            var basePath = context.Request.ApplicationPath;

            var path = context.Request.Url.AbsolutePath.Substring(basePath.Length);
            path = string.IsNullOrWhiteSpace(path) ? "/" : path;

            var nancyUrl = new Url
                               {
                                   Scheme = context.Request.Url.Scheme,
                                   HostName = context.Request.Url.Host,
                                   Port = context.Request.Url.Port,
                                   BasePath = basePath,
                                   Path = path,
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
            foreach (var header in response.Headers.ToDictionary(x => x.Key, x => x.Value))
            {
                context.Response.AddHeader(header.Key, header.Value);
            }

            foreach(var cookie in response.Cookies.ToArray())
            {
                context.Response.AddHeader("Set-Cookie", cookie.ToString());
            }
        }
    }
}