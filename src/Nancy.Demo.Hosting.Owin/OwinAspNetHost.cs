

namespace Nancy.Demo.Hosting.Owin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Routing;

    using AppAction = System.Func< // Call
        System.Collections.Generic.IDictionary<string, object>, // Environment
        System.Collections.Generic.IDictionary<string, string[]>, // Headers
        System.IO.Stream, // Body
        System.Threading.Tasks.Task<System.Tuple< //Result
            System.Collections.Generic.IDictionary<string, object>, // Properties
            int, // Status
            System.Collections.Generic.IDictionary<string, string[]>, // Headers
            System.Func< // CopyTo
                System.IO.Stream, // Body
                System.Threading.Tasks.Task>>>>; // Done


    public class OwinAspNetRouteHandler : IRouteHandler
    {
        private readonly AppAction _app;

        public OwinAspNetRouteHandler(AppAction app)
        {
            _app = app;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new OwinAspNetHandler(_app);
        }
    }

    public class OwinAspNetHandler : IHttpAsyncHandler
    {
        private readonly AppAction _app;

        public OwinAspNetHandler()
            : this(null)
        {
        }

        public OwinAspNetHandler(AppAction app)
        {
            if (app == null)
            {
                // get singleton app
                throw new NotImplementedException();
            }

            _app = app;
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotSupportedException();
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<Action>(state);
            if (callback != null)
                tcs.Task.ContinueWith(task => callback(task), TaskContinuationOptions.ExecuteSynchronously);

            var request = context.Request;
            var response = context.Response;

            var pathBase = request.ApplicationPath;
            if (pathBase == "/" || pathBase == null)
                pathBase = "";

            var path = request.Path;
            if (path.StartsWith(pathBase))
                path = path.Substring(pathBase.Length);

            var serverVarsToAddToEnv = request.ServerVariables.AllKeys
                .Where(key => !key.StartsWith("HTTP_") && !string.Equals(key, "ALL_HTTP") && !string.Equals(key, "ALL_RAW"))
                .Select(key => new KeyValuePair<string, object>(key, request.ServerVariables.Get(key)));

            var env = new Dictionary<string, object>();
            env[OwinConstants.Version] = "0.12.0";
            env[OwinConstants.RequestMethod] = request.HttpMethod;
            env[OwinConstants.RequestScheme] = request.Url.Scheme;
            env[OwinConstants.RequestPathBase] = pathBase;
            env[OwinConstants.RequestPath] = path;
            env[OwinConstants.RequestQueryString] = request.ServerVariables["QUERY_STRING"];
            env[OwinConstants.RequestProtocol] = request.ServerVariables["SERVER_PROTOCOL"];
            env["aspnet.HttpContextBase"] = context;
            env[OwinConstants.CallCompleted] = tcs.Task;

            foreach (var kv in serverVarsToAddToEnv)
                env["server." + kv.Key] = kv.Value;

            var requestHeaders = request.Headers.AllKeys
                    .ToDictionary(x => x, x => request.Headers.GetValues(x), StringComparer.OrdinalIgnoreCase);

            var requestStream = request.InputStream;

            try
            {
                _app.Invoke(env, requestHeaders, requestStream)
                    .ContinueWith(taskResultParameters =>
                    {
                        if (taskResultParameters.IsFaulted)
                        {
                            tcs.TrySetException(taskResultParameters.Exception.InnerExceptions);
                        }
                        else if (taskResultParameters.IsCanceled)
                        {
                            tcs.TrySetCanceled();
                        }
                        else
                        {
                            try
                            {
                                var resultParameters = taskResultParameters.Result;
                                var properties = resultParameters.Item1;
                                var responseStatus = resultParameters.Item2;
                                var responseHeader = resultParameters.Item3;
                                var responseCopyTo = resultParameters.Item4;

                                response.BufferOutput = false;
                                response.StatusCode = responseStatus;

                                if (responseHeader != null)
                                {
                                    foreach (var header in responseHeader)
                                    {
                                        foreach (var headerValue in header.Value)
                                            response.AddHeader(header.Key, headerValue);
                                    }
                                }

                                if (responseCopyTo != null)
                                {
                                    responseCopyTo(response.OutputStream)
                                        .ContinueWith(taskCopyTo =>
                                        {
                                            if (taskResultParameters.IsFaulted)
                                                tcs.TrySetException(taskResultParameters.Exception.InnerExceptions);
                                            else if (taskResultParameters.IsCanceled)
                                                tcs.TrySetCanceled();
                                            else
                                                tcs.TrySetResult(() => { });
                                        });
                                }
                                else
                                {
                                    // if you reach here it means you didn't implmement AppAction correctly
                                    // tcs.TrySetResult(() => { });
                                }
                            }
                            catch (Exception ex)
                            {
                                tcs.TrySetException(ex);
                            }
                        }
                    });
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        public void EndProcessRequest(IAsyncResult asyncResult)
        {
            var task = ((Task<Action>)asyncResult);
            if (task.IsFaulted)
            {
                var exception = task.Exception;
                exception.Handle(ex => ex is HttpException);
            }
            else if (task.IsCompleted)
            {
                task.Result.Invoke();
            }
        }

        internal static class OwinConstants
        {
            public const string Version = "owin.Version";
            public const string RequestScheme = "owin.RequestScheme";
            public const string RequestMethod = "owin.RequestMethod";
            public const string RequestPathBase = "owin.RequestPathBase";
            public const string RequestPath = "owin.RequestPath";
            public const string RequestQueryString = "owin.RequestQueryString";
            public const string RequestProtocol = "owin.RequestProtocol";
            public const string ReasonPhrase = "owin.ReasonPhrase";
            public const string CallCompleted = "owin.CallCompleted";
        }
    }
}