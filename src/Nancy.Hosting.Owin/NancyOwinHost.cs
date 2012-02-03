namespace Nancy.Hosting.Owin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Bootstrapper;
    using Extensions;
    using ResultDelegate = global::Owin.ResultDelegate;
    using BodyDelegate = global::Owin.BodyDelegate;

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
        /// OWIN Application Delegate
        /// </summary>
        /// <param name="environment">Application environment</param>
        /// <param name="responseCallBack">Response callback delegate</param>
        /// <param name="errorCallback">Error callback delegate</param>
        public void ProcessRequest(IDictionary<string, object> environment, ResultDelegate responseCallBack, Action<Exception> errorCallback)
        {
            CheckVersion(environment);

            var parameters = environment.AsNancyRequestParameters();

            var requestBodyDelegate = GetRequestBodyDelegate(environment);

            // If there's no body, just invoke Nancy immediately
            if (requestBodyDelegate == null)
            {
                this.InvokeNancy(parameters, responseCallBack, errorCallback);
                return;
            }

            // If a body is present, build the RequestStream and 
            // invoke Nancy when it's ready.
            requestBodyDelegate.Invoke(
                data =>
                {
                    parameters.Body.Write(data.Array, data.Offset, data.Count);
                    return false;
                },
                _ => false,
                ex =>
                {
                    if (ex != null)
                    {
                        errorCallback(ex);
                        return;
                    }
                    this.InvokeNancy(parameters, responseCallBack, errorCallback);
                },
                CancellationToken.None);
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

        private static BodyDelegate GetRequestBodyDelegate(IDictionary<string, object> environment)
        {
            return (BodyDelegate)environment["owin.RequestBody"];
        }

        private void InvokeNancy(NancyRequestParameters parameters, ResultDelegate responseCallBack, Action<Exception> errorCallback)
        {
            try
            {
                parameters.Body.Seek(0, SeekOrigin.Begin);

                var request = new Request(parameters.Method, parameters.Url, parameters.Body, parameters.Headers);

                // Execute the nancy async request handler
                this.engine.HandleRequest(
                    request,
                    (result) =>
                    {
                        var responseHeaders = GetResponseHeaders(result.Response.Headers);
                        if (!string.IsNullOrWhiteSpace(result.Response.ContentType))
                        {
                            responseHeaders["Content-Type"] = new[] { result.Response.ContentType };
                        }
                        if (result.Response.Cookies.Count > 0)
                        {
                            responseHeaders["Set-Cookie"] = result.Response.Cookies.Select(cookie => cookie.ToString());
                        }
                        responseCallBack.Invoke(GetReturnCode(result), responseHeaders, GetResponseBodyBuilder(result));
                    },
                    errorCallback);
            }
            catch (Exception e)
            {
                errorCallback.Invoke(e);
            }
        }

        private static IDictionary<string, IEnumerable<string>> GetResponseHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            return headers.ToDictionary<KeyValuePair<string, string>, string, IEnumerable<string>>(
                kv => kv.Key, 
                kv => new[] { kv.Value });
        }

        private static BodyDelegate GetResponseBodyBuilder(NancyContext result)
        {
            return (write, flush, end, cancel) =>
                {
                    var callEnd = CallOnce(end);
                    var callDispose = CallOnce(result.Dispose);

                    // Don't currently support cancelling, but if it gets called then dispose the context
                    cancel.Register(callDispose);

                    // Wrap the completion delegate so the context is disposed on completion.
                    // Technically we could just do this after the .Invoke below, but doing it
                    // here gives scope for supporting async response body generation in the future.
                    Action<Exception> onComplete = ex =>
                            {
                                callEnd(ex);
                                callDispose();
                            };

                    using (var stream = new ResponseStream(write, flush, onComplete))
                    {
                        try
                        {
                            result.Response.Contents.Invoke(stream);
                        }
                        catch (Exception e)
                        {
                            callEnd(e);
                            callDispose();
                        }
                    }
                };
        }

        public static Action CallOnce(Action call)
        {
            return () => Interlocked.Exchange(ref call, () => { }).Invoke();
        }

        public static Action<T1> CallOnce<T1>(Action<T1> call)
        {
            return t1 => Interlocked.Exchange(ref call, tt1 => { }).Invoke(t1);
        }

        private static string GetReturnCode(NancyContext result)
        {
            return String.Format("{0} {1}", (int)result.Response.StatusCode, result.Response.StatusCode);
        }

        /// <summary>
        /// State object for async request builder stream begin/endwrite
        /// </summary>
        private sealed class AsyncBuilderState
        {
            public Stream Stream { get; private set; }
            public Action OnComplete { get; private set; }
            public Action<Exception> OnError { get; private set; }

            public AsyncBuilderState(Stream stream, Action onComplete, Action<Exception> onError)
            {
                this.Stream = stream;
                this.OnComplete = onComplete;
                this.OnError = onError;
            }
        }
    }
}
