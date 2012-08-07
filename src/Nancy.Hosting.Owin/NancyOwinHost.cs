namespace Nancy.Hosting.Owin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Bootstrapper;
    using Extensions;

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

    using BodyDelegate = System.Func<System.Func<System.ArraySegment<byte>, // data
                                     System.Action,                         // continuation
                                     bool>,                                 // continuation will be invoked
                                     System.Action<System.Exception>,       // onError
                                     System.Action,                         // on Complete
                                     System.Action>;                        // cancel

   
    using ResponseCallBack = System.Action<string, System.Collections.Generic.IDictionary<string, string>, System.Func<System.Func<System.ArraySegment<byte>, System.Action, bool>, System.Action<System.Exception>, System.Action, System.Action>>;

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
        public void ProcessRequest(IDictionary<string, object> environment, ResponseCallBack responseCallBack, Action<Exception> errorCallback)
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
                GetRequestBodyBuilder(parameters, errorCallback),
                errorCallback,
                () => this.InvokeNancy(parameters, responseCallBack, errorCallback));
        }

        private static void CheckVersion(IDictionary<string, object> environment)
        {
            object version;
            environment.TryGetValue("owin.Version", out version);

            if (version != null)
            {
                var split = version.ToString().Split('.');
                int major = int.Parse(split[0]);
                int minor = split.Length > 1 ? int.Parse(split[1]) : 0;
                int patch = split.Length > 2 ? int.Parse(split[2]) : 0;

                if (major >= 1)
                {
                    // if owin.Version >= 1.x.x
                    return;
                }
                else if (minor >= 12)
                {
                    // if owin.Version >= 0.12.x
                    return;
                }
            }

            throw new InvalidOperationException("An OWIN v0.12 or higher host is required");
        }

        private static BodyDelegate GetRequestBodyDelegate(IDictionary<string, object> environment)
        {
            return (BodyDelegate)environment["owin.RequestBody"];
        }

        private static Func<ArraySegment<byte>, Action, bool> GetRequestBodyBuilder(NancyRequestParameters parameters, Action<Exception> errorCallback)
        {
            return (data, continuation) =>
                {
                    if (continuation == null)
                    {
                        // If continuation is null then we must use sync and return false
                        parameters.Body.Write(data.Array, data.Offset, data.Count);
                        return false;
                    }

                    // Otherwise use begin/end (which may be blocking anyway)
                    // and return true.
                    // No need to do any locking because the spec states we can't be called again
                    // until we call the continuation.
                    var asyncState = new AsyncBuilderState(parameters.Body, continuation, errorCallback);
                    parameters.Body.BeginWrite(
                        data.Array,
                        data.Offset,
                        data.Count,
                        (ar) =>
                        {
                            var state = (AsyncBuilderState)ar.AsyncState;

                            try
                            {
                                state.Stream.EndWrite(ar);

                                state.OnComplete.Invoke();
                            }
                            catch (Exception e)
                            {
                                state.OnError.Invoke(e);
                            }
                        },
                        asyncState);

                    return true;
                };
        }

        private void InvokeNancy(NancyRequestParameters parameters, ResponseCallBack responseCallBack, Action<Exception> errorCallback)
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
                        if (result.Response.Cookies.Count > 0)
                        {
                            result.Response.Headers["Set-Cookie"] = result.Response.GetAllCookies();
                        }
                        result.Response.Headers["Content-Type"] = result.Response.ContentType;
                        responseCallBack.Invoke(GetReturnCode(result), result.Response.Headers, GetResponseBodyBuilder(result));
                    },
                    errorCallback);
            }
            catch (Exception e)
            {
                errorCallback.Invoke(e);
            }
        }

        private static BodyDelegate GetResponseBodyBuilder(NancyContext result)
        {
            return (next, error, complete) =>
                {
                    // Wrap the completion delegate so the context is disposed on completion.
                    // Technically we could just do this after the .Invoke below, but doing it
                    // here gives scope for supporting async response body generation in the future.
                    Action onComplete = () =>
                            {
                                complete.Invoke();
                                result.Dispose();
                            };

                    using (var stream = new ResponseStream(next, onComplete))
                    {
                        try
                        {
                            result.Response.Contents.Invoke(stream);
                        }
                        catch (Exception e)
                        {
                            error.Invoke(e);
                            result.Dispose();
                        }
                    }

                    // Don't currently support cancelling, but if it gets called then dispose the context
                    return result.Dispose;
                };
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
