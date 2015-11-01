namespace Nancy.Hosting.Self
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Principal;

    using Nancy.Bootstrapper;
    using Nancy.Extensions;
    using Nancy.Helpers;
    using Nancy.IO;

    /// <summary>
    /// Allows to host Nancy server inside any application - console or windows service.
    /// </summary>
    /// <remarks>
    /// NancyHost uses <see cref="System.Net.HttpListener"/> internally. Therefore, it requires full .net 4.0 profile (not client profile)
    /// to run. <see cref="Start"/> will launch a thread that will listen for requests and then process them. Each request is processed in
    /// its own execution thread. NancyHost needs <see cref="SerializableAttribute"/> in order to be used from another appdomain under
    /// mono. Working with AppDomains is necessary if you want to unload the dependencies that come with NancyHost.
    /// </remarks>
    [Serializable]
    public class NancyHost : IDisposable
    {
        private const int ACCESS_DENIED = 5;

        private readonly IList<Uri> baseUriList;
        private HttpListener listener;
        private readonly INancyEngine engine;
        private readonly HostConfiguration configuration;
        private readonly INancyBootstrapper bootstrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyHost"/> class for the specified <paramref name="baseUris"/>.
        /// Uses the default configuration
        /// </summary>
        /// <param name="baseUris">The <see cref="Uri"/>s that the host will listen to.</param>
        public NancyHost(params Uri[] baseUris)
            : this(NancyBootstrapperLocator.Bootstrapper, new HostConfiguration(), baseUris) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyHost"/> class for the specified <paramref name="baseUris"/>.
        /// Uses the specified configuration.
        /// </summary>
        /// <param name="baseUris">The <see cref="Uri"/>s that the host will listen to.</param>
        /// <param name="configuration">Configuration to use</param>
        public NancyHost(HostConfiguration configuration, params Uri[] baseUris)
            : this(NancyBootstrapperLocator.Bootstrapper, configuration, baseUris){}

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyHost"/> class for the specified <paramref name="baseUris"/>, using
        /// the provided <paramref name="bootstrapper"/>.
        /// Uses the default configuration
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper that should be used to handle the request.</param>
        /// <param name="baseUris">The <see cref="Uri"/>s that the host will listen to.</param>
        public NancyHost(INancyBootstrapper bootstrapper, params Uri[] baseUris)
            : this(bootstrapper, new HostConfiguration(), baseUris)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyHost"/> class for the specified <paramref name="baseUris"/>, using
        /// the provided <paramref name="bootstrapper"/>.
        /// Uses the specified configuration.
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper that should be used to handle the request.</param>
        /// <param name="configuration">Configuration to use</param>
        /// <param name="baseUris">The <see cref="Uri"/>s that the host will listen to.</param>
        public NancyHost(INancyBootstrapper bootstrapper, HostConfiguration configuration, params Uri[] baseUris)
        {
            this.bootstrapper = bootstrapper;
            this.configuration = configuration ?? new HostConfiguration();
            this.baseUriList = baseUris;

            bootstrapper.Initialise();
            this.engine = bootstrapper.GetEngine();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyHost"/> class for the specified <paramref name="baseUri"/>, using
        /// the provided <paramref name="bootstrapper"/>.
        /// Uses the default configuration
        /// </summary>
        /// <param name="baseUri">The <see cref="Uri"/> that the host will listen to.</param>
        /// <param name="bootstrapper">The bootstrapper that should be used to handle the request.</param>
        public NancyHost(Uri baseUri, INancyBootstrapper bootstrapper)
            : this(bootstrapper, new HostConfiguration(), baseUri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyHost"/> class for the specified <paramref name="baseUri"/>, using
        /// the provided <paramref name="bootstrapper"/>.
        /// Uses the specified configuration.
        /// </summary>
        /// <param name="baseUri">The <see cref="Uri"/> that the host will listen to.</param>
        /// <param name="bootstrapper">The bootstrapper that should be used to handle the request.</param>
        /// <param name="configuration">Configuration to use</param>
        public NancyHost(Uri baseUri, INancyBootstrapper bootstrapper, HostConfiguration configuration)
            : this (bootstrapper, configuration, baseUri)
        {
        }

        /// <summary>
        /// Stops the host if it is running.
        /// </summary>
        public void Dispose()
        {
            this.Stop();

            this.bootstrapper.Dispose();
        }

        /// <summary>
        /// Start listening for incoming requests with the given configuration
        /// </summary>
        public void Start()
        {
            this.StartListener();

            try
            {
                this.listener.BeginGetContext(this.GotCallback, null);
            }
            catch (Exception e)
            {
                this.configuration.UnhandledExceptionCallback.Invoke(e);

                throw;
            }
        }

        private void StartListener()
        {
            if (this.TryStartListener())
            {
                return;
            }

            if (!this.configuration.UrlReservations.CreateAutomatically)
            {
                throw new AutomaticUrlReservationCreationFailureException(this.GetPrefixes(), this.GetUser());
            }

            if (!this.TryAddUrlReservations())
            {
                throw new InvalidOperationException("Unable to configure namespace reservation");
            }

            if (!TryStartListener())
            {
                throw new InvalidOperationException("Unable to start listener");
            }
        }

        private bool TryStartListener()
        {
            try
            {
                // if the listener fails to start, it gets disposed;
                // so we need a new one, each time.
                this.listener = new HttpListener();
                foreach (var prefix in this.GetPrefixes())
                {
                    this.listener.Prefixes.Add(prefix);
                }

                this.listener.Start();

                return true;
            }
            catch (HttpListenerException e)
            {
                if (e.ErrorCode == ACCESS_DENIED)
                {
                    return false;
                }

                throw;
            }
        }

        private bool TryAddUrlReservations()
        {
            var user = this.GetUser();

            foreach (var prefix in this.GetPrefixes())
            {
                if (!NetSh.AddUrlAcl(prefix, user))
                {
                    return false;
                }
            }

            return true;
        }

        private string GetUser()
        {
            if (!string.IsNullOrWhiteSpace(this.configuration.UrlReservations.User))
            {
                return this.configuration.UrlReservations.User;
            }

            return WindowsIdentity.GetCurrent().Name;
        }

        /// <summary>
        /// Stop listening for incoming requests.
        /// </summary>
        public void Stop()
        {
            if (this.listener.IsListening)
            {
                listener.Stop();
            }
        }

        internal IEnumerable<string> GetPrefixes()
        {
            foreach (var baseUri in this.baseUriList)
            {
                var prefix = new UriBuilder(baseUri).ToString();

                if (this.configuration.RewriteLocalhost && !baseUri.Host.Contains("."))
                {
                    prefix = prefix.Replace("localhost", "+");
                }

                yield return prefix;
            }
        }

        private Request ConvertRequestToNancyRequest(HttpListenerRequest request)
        {
            var baseUri = this.GetBaseUri(request);

            if (baseUri == null)
            {
                throw new InvalidOperationException(String.Format("Unable to locate base URI for request: {0}",request.Url));
            }

            var expectedRequestLength =
                GetExpectedRequestLength(request.Headers.ToDictionary());

            var relativeUrl = baseUri.MakeAppLocalPath(request.Url);

            var nancyUrl = new Url
            {
                Scheme = request.Url.Scheme,
                HostName = request.Url.Host,
                Port = request.Url.IsDefaultPort ? null : (int?)request.Url.Port,
                BasePath = baseUri.AbsolutePath.TrimEnd('/'),
                Path = HttpUtility.UrlDecode(relativeUrl),
                Query = request.Url.Query,
            };

            byte[] certificate = null;

            if (this.configuration.EnableClientCertificates)
            {
                var x509Certificate = request.GetClientCertificate();

                if (x509Certificate != null)
                {
                    certificate = x509Certificate.RawData;
                }
            }

            // NOTE: For HTTP/2 we want fieldCount = 1,
            // otherwise (HTTP/1.0 and HTTP/1.1) we want fieldCount = 2
            var fieldCount = request.ProtocolVersion.Major == 2 ? 1 : 2;

            var protocolVersion = string.Format("HTTP/{0}", request.ProtocolVersion.ToString(fieldCount));

            return new Request(
                request.HttpMethod,
                nancyUrl,
                RequestStream.FromStream(request.InputStream, expectedRequestLength, StaticConfiguration.DisableRequestStreamSwitching ?? false),
                request.Headers.ToDictionary(),
                (request.RemoteEndPoint != null) ? request.RemoteEndPoint.Address.ToString() : null,
                certificate,
                protocolVersion);
        }

        private Uri GetBaseUri(HttpListenerRequest request)
        {
            var result = this.baseUriList.FirstOrDefault(uri => uri.IsCaseInsensitiveBaseOf(request.Url));

            if (result != null)
            {
                return result;
            }

            if (!this.configuration.AllowAuthorityFallback)
            {
                return null;
            }

            return new Uri(request.Url.GetLeftPart(UriPartial.Authority));
        }

        private void ConvertNancyResponseToResponse(Response nancyResponse, HttpListenerResponse response)
        {
            foreach (var header in nancyResponse.Headers)
            {
                if (!IgnoredHeaders.IsIgnored(header.Key))
                {
                    response.AddHeader(header.Key, header.Value);
                }
            }

            foreach (var nancyCookie in nancyResponse.Cookies)
            {
                response.Headers.Add(HttpResponseHeader.SetCookie, nancyCookie.ToString());
            }

            if (nancyResponse.ReasonPhrase != null)
            {
                response.StatusDescription = nancyResponse.ReasonPhrase;
            }

            if (nancyResponse.ContentType != null)
            {
                response.ContentType = nancyResponse.ContentType;
            }

            response.StatusCode = (int)nancyResponse.StatusCode;

            if (configuration.AllowChunkedEncoding)
            {
                OutputWithDefaultTransferEncoding(nancyResponse, response);
            }
            else
            {
                OutputWithContentLength(nancyResponse, response);
            }
        }

        private static void OutputWithDefaultTransferEncoding(Response nancyResponse, HttpListenerResponse response)
        {
            using (var output = response.OutputStream)
            {
                nancyResponse.Contents.Invoke(output);
            }
        }

        private static void OutputWithContentLength(Response nancyResponse, HttpListenerResponse response)
        {
            byte[] buffer;
            using (var memoryStream = new MemoryStream())
            {
                nancyResponse.Contents.Invoke(memoryStream);
                buffer = memoryStream.ToArray();
            }

            var contentLength = (nancyResponse.Headers.ContainsKey("Content-Length")) ?
                Convert.ToInt64(nancyResponse.Headers["Content-Length"]) :
                buffer.Length;

            response.SendChunked = false;
            response.ContentLength64 = contentLength;

            using (var output = response.OutputStream)
            {
                using (var writer = new BinaryWriter(output))
                {
                    writer.Write(buffer);
                    writer.Flush();
                }
            }
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

            return !long.TryParse(headerValue, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength) ?
                0 :
                contentLength;
        }

        private void GotCallback(IAsyncResult ar)
        {
            try
            {
                var ctx = this.listener.EndGetContext(ar);
                this.listener.BeginGetContext(this.GotCallback, null);
                this.Process(ctx);
            }
            catch (Exception e)
            {
                this.configuration.UnhandledExceptionCallback.Invoke(e);

                try
                {
                    this.listener.BeginGetContext(this.GotCallback, null);
                }
                catch
                {
                    this.configuration.UnhandledExceptionCallback.Invoke(e);
                }
            }
        }

        private void Process(HttpListenerContext ctx)
        {
            try
            {
                var nancyRequest = this.ConvertRequestToNancyRequest(ctx.Request);
                using (var nancyContext = this.engine.HandleRequest(nancyRequest))
                {
                    try
                    {
                        ConvertNancyResponseToResponse(nancyContext.Response, ctx.Response);
                    }
                    catch (Exception e)
                    {
                        this.configuration.UnhandledExceptionCallback.Invoke(e);
                    }
                }
            }
            catch (Exception e)
            {
                this.configuration.UnhandledExceptionCallback.Invoke(e);
            }
        }
    }
}
