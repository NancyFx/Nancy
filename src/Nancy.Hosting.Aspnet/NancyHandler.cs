namespace Nancy.Hosting.Aspnet
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
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
        /// <param name="cb"></param>
        /// <param name="state"></param>
        public Task<Tuple<NancyContext, HttpContextBase>> ProcessRequest(HttpContextBase context, AsyncCallback cb, object state)
        {
            var request = CreateNancyRequest(context);

            var tcs = new TaskCompletionSource<Tuple<NancyContext, HttpContextBase>>(state);

            if (cb != null)
            {
                tcs.Task.ContinueWith(task => cb(task), TaskContinuationOptions.ExecuteSynchronously);
            }

            this.engine.HandleRequest(
                request, 
                ctx => tcs.SetResult(new Tuple<NancyContext, HttpContextBase>(ctx, context)), 
                tcs.SetException);

            return tcs.Task;
        }

        public static void EndProcessRequest(Task<Tuple<NancyContext, HttpContextBase>> task)
        {
            if (task.IsFaulted)
            {
                var exception = task.Exception;
                exception.Handle(ex => ex is HttpException);
            }

            var nancyContext = task.Result.Item1;
            var httpContext = task.Result.Item2;

            NancyHandler.SetNancyResponseToHttpResponse(httpContext, nancyContext.Response);
            nancyContext.Dispose();
        }

        private static Request CreateNancyRequest(HttpContextBase context)
        {
            var incomingHeaders = context.Request.Headers.ToDictionary();

            var expectedRequestLength =
                GetExpectedRequestLength(incomingHeaders);

            var basePath = context.Request.ApplicationPath.TrimEnd('/');

            var url = context.Request.Url;
            var path = ResolvePath(url, basePath);

            var nancyUrl = new Url
                               {
                                   Scheme = url.Scheme,
                                   HostName = url.Host,
                                   Port = url.Port,
                                   BasePath = basePath,
                                   Path = path,
                                   Query = url.Query,
                               };
            byte[] certificate = null;

            if (context.Request.ClientCertificate != null &&
                context.Request.ClientCertificate.IsPresent &&
                context.Request.ClientCertificate.Certificate.Length != 0)
            {
                certificate = context.Request.ClientCertificate.Certificate;
            }
                
            return new Request(
                context.Request.HttpMethod.ToUpperInvariant(),
                nancyUrl,
                RequestStream.FromStream(context.Request.InputStream, expectedRequestLength, true),
                incomingHeaders,
                context.Request.UserHostAddress,
                certificate);
        }

        private static string ResolvePath(Uri url, string basePath)
        {
            const string fragmentRegex = "#.*$";
            const string queryRegex = @"\?.*$";
            var portRegex = "^:" + url.Port;

            var originalString = url.OriginalString;
            var path = originalString.Substring(originalString.IndexOf(url.Host, StringComparison.OrdinalIgnoreCase) + url.Host.Length);
            path = Regex.Replace(path, portRegex, String.Empty);
            path = Regex.Replace(path, fragmentRegex, String.Empty, RegexOptions.IgnoreCase);
            path = Regex.Replace(path, queryRegex, String.Empty, RegexOptions.IgnoreCase);

            if (path.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                path = path.Remove(0, basePath.Length);
            }

            if (!path.StartsWith("/"))
                path = "/" + path;

            return path;
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

            if (response.ReasonPhrase != null)
            {
                context.Response.StatusDescription = response.ReasonPhrase;
            }

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