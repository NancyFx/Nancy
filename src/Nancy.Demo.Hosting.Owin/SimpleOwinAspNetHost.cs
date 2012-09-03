
namespace Nancy.Demo.Hosting.Owin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Routing;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class SimpleOwinAspNetRouteHandler : IRouteHandler
    {
        private readonly SimpleOwinAspNetHandler _simpleOwinAspNetHandler;

        public SimpleOwinAspNetRouteHandler(AppFunc app)
            : this(app, null)
        {
        }

        public SimpleOwinAspNetRouteHandler(AppFunc app, string root)
        {
            _simpleOwinAspNetHandler = new SimpleOwinAspNetHandler(app, root);
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return _simpleOwinAspNetHandler;
        }
    }

    public class SimpleOwinAspNetHandler : IHttpAsyncHandler
    {
        private readonly AppFunc _appFunc;
        private readonly string _root;

        public SimpleOwinAspNetHandler(AppFunc app)
            : this(app, null)
        {
        }

        public SimpleOwinAspNetHandler(AppFunc app, string root)
        {
            if (app == null)
                throw new ArgumentNullException("app");

            _appFunc = app;
            if (!string.IsNullOrWhiteSpace(root))
            {
                if (!root.StartsWith("/"))
                    _root += "/" + root;
            }
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
            return BeginProcessRequest(new HttpContextWrapper(context), callback, state);
        }

        public IAsyncResult BeginProcessRequest(HttpContextBase context, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<Action>(state);
            if (callback != null)
                tcs.Task.ContinueWith(task => callback(task), TaskContinuationOptions.ExecuteSynchronously);

            var request = context.Request;
            var response = context.Response;

            var pathBase = request.ApplicationPath;
            if (pathBase == "/" || pathBase == null)
                pathBase = "";

            if (_root != null)
                pathBase += _root;

            var path = request.Path;
            if (path.StartsWith(pathBase))
                path = path.Substring(pathBase.Length);

            var serverVarsToAddToEnv = request.ServerVariables.AllKeys
                .Where(key => !key.StartsWith("HTTP_") && !string.Equals(key, "ALL_HTTP") && !string.Equals(key, "ALL_RAW"))
                .Select(key => new KeyValuePair<string, object>(key, request.ServerVariables.Get(key)));

            var env = new Dictionary<string, object>();
            env[OwinConstants.Version] = "1.0";
            env[OwinConstants.RequestMethod] = request.HttpMethod;
            env[OwinConstants.RequestScheme] = request.Url.Scheme;
            env[OwinConstants.RequestPathBase] = pathBase;
            env[OwinConstants.RequestPath] = path;
            env[OwinConstants.RequestQueryString] = request.ServerVariables["QUERY_STRING"];
            env[OwinConstants.RequestProtocol] = request.ServerVariables["SERVER_PROTOCOL"];
            env[OwinConstants.RequestBody] = request.InputStream;
            env[OwinConstants.RequestHeaders] = request.Headers.AllKeys
                    .ToDictionary(x => x, x => request.Headers.GetValues(x), StringComparer.OrdinalIgnoreCase);

            env[OwinConstants.CallCancelled] = CancellationToken.None;

            env[OwinConstants.ResponseHeaders] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            env[OwinConstants.ResponseBody] =
                new TriggerStream(response.OutputStream)
                {
                    OnFirstWrite = () =>
                    {
                        response.StatusCode = Get<int>(env, OwinConstants.ResponseStatusCode, 200);

                        object reasonPhrase;
                        if (env.TryGetValue(OwinConstants.ResponseReasonPhrase, out reasonPhrase))
                            response.StatusDescription = Convert.ToString(reasonPhrase);

                        var responseHeaders = Get<IDictionary<string, string[]>>(env, OwinConstants.ResponseHeaders, null);
                        if (responseHeaders != null)
                        {
                            foreach (var responseHeader in responseHeaders)
                            {
                                foreach (var headerValue in responseHeader.Value)
                                    response.AddHeader(responseHeader.Key, headerValue);
                            }
                        }
                    }
                };

            SetEnvironmentServerVariables(env, serverVarsToAddToEnv);

            env["aspnet.HttpContextBase"] = context;

            response.BufferOutput = false;

            try
            {
                _appFunc(env)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted) tcs.TrySetException(t.Exception.InnerExceptions);
                        else if (t.IsCanceled) tcs.TrySetCanceled();
                        else tcs.TrySetResult(() => { });
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

        [DebuggerStepThrough]
        private static void SetEnvironmentServerVariables(Dictionary<string, object> env, IEnumerable<KeyValuePair<string, object>> serverVarsToAddToEnv)
        {
            foreach (var kv in serverVarsToAddToEnv)
                env["server." + kv.Key] = kv.Value;
        }

        private static T Get<T>(IDictionary<string, object> env, string key, T defaultValue)
        {
            object value;
            return env.TryGetValue(key, out value) && value is T ? (T)value : defaultValue;
        }

        public static IDictionary<string, object> GetStartupProperties()
        {
            var properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            properties[OwinConstants.Version] = "1.0";
            return properties;
        }

        private static class OwinConstants
        {
            public const string Version = "owin.Version";

            public const string RequestScheme = "owin.RequestScheme";
            public const string RequestMethod = "owin.RequestMethod";
            public const string RequestPathBase = "owin.RequestPathBase";
            public const string RequestPath = "owin.RequestPath";
            public const string RequestQueryString = "owin.RequestQueryString";
            public const string RequestProtocol = "owin.RequestProtocol";
            public const string RequestHeaders = "owin.RequestHeaders";
            public const string RequestBody = "owin.RequestBody";

            public const string CallCancelled = "owin.CallCancelled";

            public const string ResponseStatusCode = "owin.ResponseStatusCode";
            public const string ResponseReasonPhrase = "owin.ResponseReasonPhrase";
            public const string ResponseHeaders = "owin.ResponseHeaders";
            public const string ResponseBody = "owin.ResponseBody";

            public const string WebSocketSupport = "websocket.Support";
            public const string WebSocketBodyDelegte = "websocket.Func";
        }

        /// <remarks>TriggerStream pulled from Gate source code.</remarks>
        private class TriggerStream : Stream
        {
            public TriggerStream(Stream innerStream)
            {
                InnerStream = innerStream;
            }

            public Stream InnerStream { get; set; }

            public Action OnFirstWrite { get; set; }

            private bool IsStarted { get; set; }

            public override bool CanRead
            {
                get { return InnerStream.CanRead; }
            }

            public override bool CanWrite
            {
                get { return InnerStream.CanWrite; }
            }

            public override bool CanSeek
            {
                get { return InnerStream.CanSeek; }
            }

            public override bool CanTimeout
            {
                get { return InnerStream.CanTimeout; }
            }

            public override int WriteTimeout
            {
                get { return InnerStream.WriteTimeout; }
                set { InnerStream.WriteTimeout = value; }
            }

            public override int ReadTimeout
            {
                get { return InnerStream.ReadTimeout; }
                set { InnerStream.ReadTimeout = value; }
            }

            public override long Position
            {
                get { return InnerStream.Position; }
                set { InnerStream.Position = value; }
            }

            public override long Length
            {
                get { return InnerStream.Length; }
            }

            public override void Close()
            {
                InnerStream.Close();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    InnerStream.Dispose();
                }
            }

            public override string ToString()
            {
                return InnerStream.ToString();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return InnerStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                InnerStream.SetLength(value);
            }

            public override int ReadByte()
            {
                return InnerStream.ReadByte();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return InnerStream.Read(buffer, offset, count);
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return InnerStream.BeginRead(buffer, offset, count, callback, state);
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                return InnerStream.EndRead(asyncResult);
            }

            public override void WriteByte(byte value)
            {
                Start();
                InnerStream.WriteByte(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                Start();
                InnerStream.Write(buffer, offset, count);
            }

            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                Start();
                return InnerStream.BeginWrite(buffer, offset, count, callback, state);
            }

            public override void EndWrite(IAsyncResult asyncResult)
            {
                InnerStream.EndWrite(asyncResult);
            }

            public override void Flush()
            {
                Start();
                InnerStream.Flush();
            }

            private void Start()
            {
                if (!IsStarted)
                {
                    IsStarted = true;
                    if (OnFirstWrite != null)
                    {
                        OnFirstWrite();
                    }
                }
            }
        }
    }
}