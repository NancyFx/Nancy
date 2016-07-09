namespace Nancy.Hosting.Aspnet
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using System.Web;

    using Nancy.Extensions;
    using Nancy.IO;

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
        /// <param name="httpContext">The <see cref="HttpContextBase"/> of the request.</param>
        public async Task ProcessRequest(HttpContextBase httpContext)
        {
            var request = CreateNancyRequest(httpContext);

            using(var nancyContext = await this.engine.HandleRequest(request).ConfigureAwait(false))
            {
                SetNancyResponseToHttpResponse(httpContext, nancyContext.Response);
            }
        }

        private static Request CreateNancyRequest(HttpContextBase context)
        {
            var incomingHeaders = context.Request.Headers.ToDictionary();

            var expectedRequestLength =
                GetExpectedRequestLength(incomingHeaders);

            var basePath = context.Request.ApplicationPath.TrimEnd('/');

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
                               };
            byte[] certificate = null;

            if (context.Request.ClientCertificate != null &&
                context.Request.ClientCertificate.IsPresent &&
                context.Request.ClientCertificate.Certificate.Length != 0)
            {
                certificate = context.Request.ClientCertificate.Certificate;
            }

            RequestStream body = null;

            if (expectedRequestLength != 0)
            {
                body = RequestStream.FromStream(context.Request.InputStream, expectedRequestLength, StaticConfiguration.DisableRequestStreamSwitching ?? true);
            }

            var protocolVersion = context.Request.ServerVariables["HTTP_VERSION"];

            return new Request(context.Request.HttpMethod.ToUpperInvariant(), 
                nancyUrl, 
                body, 
                incomingHeaders, 
                context.Request.UserHostAddress, 
                new X509Certificate2(certificate),
                protocolVersion);
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

        public static void SetNancyResponseToHttpResponse(HttpContextBase context, Response response)
        {
            SetHttpResponseHeaders(context, response);

            if (response.ContentType != null)
            {
                context.Response.ContentType = response.ContentType;
            }

            if (IsOutputBufferDisabled())
            {
                context.Response.BufferOutput = false;
            }

            context.Response.StatusCode = (int) response.StatusCode;

            if (response.ReasonPhrase != null)
            {
                context.Response.StatusDescription = response.ReasonPhrase;
            }

            response.Contents.Invoke(new NancyResponseStream(context.Response));
        }

        private static bool IsOutputBufferDisabled()
        {
            var configurationSection =
                ConfigurationManager.GetSection("nancyFx") as NancyFxSection;

            if (configurationSection == null || configurationSection.DisableOutputBuffer == null)
            {
                return false;
            }

            return configurationSection.DisableOutputBuffer.Value;
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