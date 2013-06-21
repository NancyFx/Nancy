namespace Nancy.Owin
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Security.Cryptography.X509Certificates;

    using Nancy.Bootstrapper;
    using Nancy.IO;

    /// <summary>
    /// Nancy host for OWIN hosts
    /// </summary>
    public class NancyOwinHost
    {
        private readonly HostConfiguration hostConfiguration;

        private readonly INancyEngine engine;

        public const string RequestEnvironmentKey = "OWIN_REQUEST_ENVIRONMENT";

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyOwinHost"/> class.
        /// </summary>
        /// <param name="next">Next middleware to run if necessary</param>
        /// <param name="bootstrapper">The bootstrapper that should be used by the host.</param>
        /// <param name="hostConfiguration">Host configuration options</param>
        public NancyOwinHost(Func<IDictionary<string, object>, Task> next, INancyBootstrapper bootstrapper, HostConfiguration hostConfiguration)
        {
            this.hostConfiguration = hostConfiguration;
            bootstrapper.Initialise();

            this.engine = bootstrapper.GetEngine();
        }

        /// <summary>
        /// OWIN App Action
        /// </summary>
        /// <param name="environment">Application environment</param>
        /// <returns>Returns result</returns>
        public Task Invoke(IDictionary<string, object> environment)
        {
            var owinRequestMethod = environment.Get<string>("owin.RequestMethod");
            var owinRequestScheme = environment.Get<string>("owin.RequestScheme");
            var owinRequestHeaders = environment.Get<IDictionary<string, string[]>>("owin.RequestHeaders");
            var owinRequestPathBase = environment.Get<string>("owin.RequestPathBase");
            var owinRequestPath = environment.Get<string>("owin.RequestPath");
            var owinRequestQueryString = environment.Get<string>("owin.RequestQueryString");
            var owinRequestBody = environment.Get<Stream>("owin.RequestBody");
            var owinRequestHost = owinRequestHeaders.GetHeader("Host") ?? "localhost"; //Host header may not be present in pure owin environment

            byte[] certificate = null;
            if (this.hostConfiguration.EnableClientCertificates)
            {
                var clientCertificate = environment.Get<X509Certificate>("ssl.ClientCertificate");
                certificate = (clientCertificate == null) ? null : clientCertificate.GetRawCertData();
            }

            var serverClientIp = environment.Get<string>("server.RemoteIpAddress");

            //var callCancelled = Get<CancellationToken>(environment, "owin.RequestBody");

            var url = CreateUrl(owinRequestHost, owinRequestScheme, owinRequestPathBase, owinRequestPath, owinRequestQueryString);

            var nancyRequestStream = new RequestStream(owinRequestBody, ExpectedLength(owinRequestHeaders), false);

            var nancyRequest = new Request(
                    owinRequestMethod,
                    url,
                    nancyRequestStream,
                    owinRequestHeaders.ToDictionary(kv => kv.Key, kv => (IEnumerable<string>)kv.Value, StringComparer.OrdinalIgnoreCase),
                    serverClientIp,
                    certificate);

            var tcs = new TaskCompletionSource<int>();

            this.engine.HandleRequest(
                nancyRequest,
                StoreEnvironment(environment),
                RequestComplete(environment, tcs), 
                RequestErrored(tcs));

            return tcs.Task;
        }

        /// <summary>
        /// Creates the Nancy URL
        /// </summary>
        /// <param name="owinRequestHost">OWIN Hostname</param>
        /// <param name="owinRequestScheme">OWIN Scheme</param>
        /// <param name="owinRequestPathBase">OWIN Base path</param>
        /// <param name="owinRequestPath">OWIN Path</param>
        /// <param name="owinRequestQueryString">OWIN Querystring</param>
        /// <returns></returns>
        private static Url CreateUrl(
            string owinRequestHost,
            string owinRequestScheme,
            string owinRequestPathBase,
            string owinRequestPath,
            string owinRequestQueryString)
        {
            int? port = null;

            var hostnameParts = owinRequestHost.Split(':');
            if (hostnameParts.Length == 2)
            {
                owinRequestHost = hostnameParts[0];

                int tempPort;
                if (int.TryParse(hostnameParts[1], out tempPort))
                {
                    port = tempPort;
                }
            }

            var url = new Url
                          {
                              Scheme = owinRequestScheme,
                              HostName = owinRequestHost,
                              Port = port,
                              BasePath = owinRequestPathBase,
                              Path = owinRequestPath,
                              Query = owinRequestQueryString,
                          };
            return url;
        }

        /// <summary>
        /// Gets a delegate to store the OWIN environment into the NancyContext
        /// </summary>
        /// <param name="environment">OWIN Environment</param>
        /// <returns>Delegate</returns>
        private static Func<NancyContext, NancyContext> StoreEnvironment(IDictionary<string, object> environment)
        {
            return context =>
                {
                    environment["nancy.NancyContext"] = context;
                    context.Items[RequestEnvironmentKey] = environment;
                    return context;
                };
        }

        /// <summary>
        /// Gets a delegate to handle converting a nancy response
        /// to the format required by OWIN and signals that the we are
        /// now complete.
        /// </summary>
        /// <param name="environment">OWIN environment</param>
        /// <param name="tcs">The task completion source to signal</param>
        /// <returns>Delegate</returns>
        private static Action<NancyContext> RequestComplete(IDictionary<string, object> environment, TaskCompletionSource<int> tcs)
        {
            return context =>
                {
                    var owinResponseHeaders = environment.Get<IDictionary<string, string[]>>("owin.ResponseHeaders");
                    var owinResponseBody = environment.Get<Stream>("owin.ResponseBody");

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

                    nancyResponse.Contents(owinResponseBody);

                    context.Dispose();

                    tcs.TrySetResult(0);
                };
        }

        /// <summary>
        /// Gets a delegate to handle request errors
        /// </summary>
        /// <param name="tcs">Completion source to signal</param>
        /// <returns>Delegate</returns>
        private Action<Exception> RequestErrored(TaskCompletionSource<int> tcs)
        {
            return tcs.SetException;
        }

        private static long ExpectedLength(IDictionary<string, string[]> headers)
        {
            var header = headers.GetHeader("Content-Length");
            if (string.IsNullOrWhiteSpace(header))
                return 0;

            int contentLength;
            return int.TryParse(header, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength) ? contentLength : 0;
        }
    }
}
