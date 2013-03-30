namespace Nancy.Owin
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Security.Cryptography.X509Certificates;

    using Nancy.IO;

    /// <summary>
    /// Nancy host for OWIN hosts
    /// </summary>
    public class NancyOwinHost
    {
        private readonly Func<IDictionary<string, object>, Task> next;
        private readonly INancyEngine engine;
        private readonly HashSet<HttpStatusCode> passThroughStatusCodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyOwinHost"/> class.
        /// </summary>
        /// <param name="next">Next middleware to run if necessary</param>
        /// <param name="options">The nancy options that should be used by the host.</param>
        public NancyOwinHost(Func<IDictionary<string, object>, Task> next, NancyOptions options)
        {
            this.next = next;
            this.passThroughStatusCodes = new HashSet<HttpStatusCode>(options.PassThroughStatusCodes);
            options.Bootstrapper.Initialise();
            this.engine = options.Bootstrapper.GetEngine();
        }

        /// <summary>
        /// The Entry point when hosting Nancy in an OWIN application
        /// </summary>
        /// <param name="environment">OWIN environment dictionary</param>
        /// <returns>Returns result</returns>
        public Task Invoke(IDictionary<string, object> environment)
        {
            var owinRequestMethod = Get<string>(environment, "owin.RequestMethod");
            var owinRequestScheme = Get<string>(environment, "owin.RequestScheme");
            var owinRequestHeaders = Get<IDictionary<string, string[]>>(environment, "owin.RequestHeaders");
            var owinRequestPathBase = Get<string>(environment, "owin.RequestPathBase");
            var owinRequestPath = Get<string>(environment, "owin.RequestPath");
            var owinRequestQueryString = Get<string>(environment, "owin.RequestQueryString");
            var owinRequestBody = Get<Stream>(environment, "owin.RequestBody");

            var clientCertificate = Get<X509Certificate>(environment, "ssl.ClientCertificate");
            var certificate = (clientCertificate == null) ? null : clientCertificate.GetRawCertData();

            var serverClientIp = Get<string>(environment, "server.RemoteIpAddress");

            //var callCancelled = Get<CancellationToken>(environment, "owin.RequestBody");

            var url = new Url
            {
                Scheme = owinRequestScheme,
                HostName = GetHeader(owinRequestHeaders, "Host"),
                Port = null,
                BasePath = owinRequestPathBase,
                Path = owinRequestPath,
                Query = owinRequestQueryString,
            };

            var nancyRequestStream = new RequestStream(owinRequestBody, ExpectedLength(owinRequestHeaders), false);

            var nancyRequest = new Request(
                    owinRequestMethod,
                    url,
                    nancyRequestStream,
                    owinRequestHeaders.ToDictionary(kv => kv.Key, kv => (IEnumerable<string>)kv.Value, StringComparer.OrdinalIgnoreCase),
                    serverClientIp,
                    certificate);

            return this.engine.HandleRequestAsync(nancyRequest)
                .ContinueWith(t =>
                              {
                                  if (t.IsFaulted)
                                  {
                                      return TaskHelpers.FromError(t.Exception);
                                  }
                                  return RequestComplete(t.Result, environment);
                              });
        }

        private Task RequestComplete(
            NancyContext context,
            IDictionary<string, object> environment)
        {
            var owinResponseHeaders = Get<IDictionary<string, string[]>>(environment, "owin.ResponseHeaders");
            var owinResponseBody = Get<Stream>(environment, "owin.ResponseBody");

            var nancyResponse = context.Response;
            environment["owin.ResponseStatusCode"] = (int)nancyResponse.StatusCode;

            foreach (var responseHeader in nancyResponse.Headers)
            {
                owinResponseHeaders[responseHeader.Key] = new[] { responseHeader.Value };
            }

            if (!string.IsNullOrWhiteSpace(nancyResponse.ContentType))
            {
                owinResponseHeaders["Content-Type"] = new[] { nancyResponse.ContentType };
            }

            if (nancyResponse.Cookies != null && nancyResponse.Cookies.Count != 0)
            {
                owinResponseHeaders["Set-Cookie"] =
                    nancyResponse.Cookies.Select(cookie => cookie.ToString()).ToArray();
            }
            if (passThroughStatusCodes.Contains(nancyResponse.StatusCode))
            {
                return next(environment);
            }
            nancyResponse.Contents(owinResponseBody);
            context.Dispose();
            return TaskHelpers.Completed();
        }

        private static T Get<T>(IDictionary<string, object> env, string key)
        {
            object value;
            return env.TryGetValue(key, out value) && value is T ? (T)value : default(T);
        }

        private static string GetHeader(IDictionary<string, string[]> headers, string key)
        {
            string[] value;
            return headers.TryGetValue(key, out value) && value != null ? string.Join(",", value.ToArray()) : null;
        }

        private static long ExpectedLength(IDictionary<string, string[]> headers)
        {
            var header = GetHeader(headers, "Content-Length");
            if (string.IsNullOrWhiteSpace(header))
                return 0;

            int contentLength;
            return int.TryParse(header, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength) ? contentLength : 0;
        }
    }
}
