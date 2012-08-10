namespace Nancy.Hosting.Owin
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Bootstrapper;
    using Nancy.IO;

    // Holy big-ass delegate signature Batman!
    using AppAction = System.Func< // Call
        System.Collections.Generic.IDictionary<string, object>, // Environment
        System.Collections.Generic.IDictionary<string, string[]>, // Headers
        System.IO.Stream, // Body
        System.Threading.Tasks.Task<System.Tuple< // Result
            System.Collections.Generic.IDictionary<string, object>, // Properties
            int, // Status
            System.Collections.Generic.IDictionary<string, string[]>, // Headers
            System.Func< // CopyTo
                System.IO.Stream, // Body
                System.Threading.Tasks.Task>>>>; // Done

    using ResultTuple = System.Tuple< //Result
        System.Collections.Generic.IDictionary<string, object>, // Properties
        int, // Status
        System.Collections.Generic.IDictionary<string, string[]>, // Headers
        System.Func< // CopyTo
            System.IO.Stream, // Body
            System.Threading.Tasks.Task>>; // Done

    using BodyAction = System.Func< // CopyTo
        System.IO.Stream, // Body
        System.Threading.Tasks.Task>; // Done

    /// <summary>
    /// Nancy host for OWIN hosts
    /// </summary>
    public class NancyOwinHost
    {
        private readonly INancyEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyOwinHost"/> class.
        /// </summary>
        public NancyOwinHost()
            : this(NancyBootstrapperLocator.Bootstrapper)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyOwinHost"/> class.
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper that should be used by the host.</param>
        public NancyOwinHost(INancyBootstrapper bootstrapper)
        {
            bootstrapper.Initialise();

            this.engine = bootstrapper.GetEngine();
        }

        /// <summary>
        /// OWIN App Action
        /// </summary>
        /// <param name="environment">Application environment</param>
        /// <param name="headers">Request headers</param>
        /// <param name="body">Body</param>
        /// <returns>Returns result</returns>
        public Task<ResultTuple> ProcessRequest(IDictionary<string, object> environment, IDictionary<string, string[]> headers, Stream body)
        {
            CheckVersion(environment);

            var owinRequestMethod = Get<string>(environment, "owin.RequestMethod");
            var owinRequestScheme = Get<string>(environment, "owin.RequestScheme");
            var owinRequestHeaders = headers;
            var owinRequestPathBase = Get<string>(environment, "owin.RequestPathBase");
            var owinRequestPath = Get<string>(environment, "owin.RequestPath");
            var owinRequestQueryString = Get<string>(environment, "owin.RequestQueryString");
            var serverClientIp = Get<string>(environment, "server.CLIENT_IP");
            var callCompleted = Get<Task>(environment, "owin.CallCompleted");

            var url = new Url
            {
                Scheme = owinRequestScheme,
                HostName = GetHeader(owinRequestHeaders, "Host"),
                Port = null,
                BasePath = owinRequestPathBase,
                Path = owinRequestPath,
                Query = owinRequestQueryString,
            };

            var nancyRequestStream = new RequestStream(body, ExpectedLength(owinRequestHeaders), false);

            var nancyRequest = new Request(
                    owinRequestMethod,
                    url,
                    nancyRequestStream,
                    owinRequestHeaders.ToDictionary(kv => kv.Key, kv => (IEnumerable<string>)kv.Value, StringComparer.OrdinalIgnoreCase),
                    serverClientIp);

            var tcs = new TaskCompletionSource<ResultTuple>();
            engine.HandleRequest(
                nancyRequest,
                context =>
                {
                    callCompleted.Finally(context.Dispose);

                    var nancyResponse = context.Response;
                    var responseHeaders = nancyResponse.Headers.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }, StringComparer.OrdinalIgnoreCase);

                    if (!string.IsNullOrWhiteSpace(nancyResponse.ContentType))
                    {
                        responseHeaders["Content-Type"] = new[] { nancyResponse.ContentType };
                    }

                    if (nancyResponse.Cookies != null && nancyResponse.Cookies.Count != 0)
                    {
                        responseHeaders["Set-Cookie"] =
                            nancyResponse.Cookies.Select(cookie => cookie.ToString()).ToArray();
                    }

                    tcs.SetResult(new ResultTuple(
                                      new Dictionary<string, object>(), // Properties
                                      (int) nancyResponse.StatusCode, // Status
                                      responseHeaders, // Headers
                                      output => // CopyTo
                                      {
                                          nancyResponse.Contents(output);
                                          return TaskHelpers.Completed();
                                      }));
                }, tcs.SetException);

            return tcs.Task;
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

        private static void CheckVersion(IDictionary<string, object> environment)
        {
            object version;
            environment.TryGetValue("owin.Version", out version);

            if (version == null || !String.Equals(version.ToString(), "1.0"))
            {
                throw new InvalidOperationException("An OWIN v1.0 host is required");
            }
        }
    }
}
