namespace Nancy.Hosting.Self
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using System.Linq;
    using IO;
    using Nancy.Bootstrapper;
    using Nancy.Cookies;
    using Nancy.Extensions;

    /// <summary>
    /// Allows to host Nancy server inside any application - console or windows service.
    /// </summary>
    /// <remarks>
    /// NancyHost uses <see cref="System.Net.HttpListener"/> internally. Therefore, it requires full .net 4.0 profile (not client profile)
    /// to run. <see cref="Start"/> will launch a thread that will listen for requests and then process them. All processing is done
    /// within a single thread - self hosting is not intended for production use, but rather as a development server.
    /// </remarks>
    public class NancyHost  
    {
        private readonly IList<Uri> baseUriList;
        private readonly HttpListener listener;
        private readonly INancyEngine engine;

        public NancyHost(params Uri[] baseUris)
            : this(NancyBootstrapperLocator.Bootstrapper, baseUris){}

        public NancyHost(INancyBootstrapper bootStrapper, params Uri[] baseUris)
        {
            baseUriList = baseUris;
            listener = new HttpListener();

            foreach (var baseUri in baseUriList)
            {
                listener.Prefixes.Add(baseUri.ToString());
            }

            bootStrapper.Initialise();
            engine = bootStrapper.GetEngine();
        }

        public NancyHost(Uri baseUri, INancyBootstrapper bootStrapper) : this (bootStrapper, baseUri) {}


        public void Start()
        {
            listener.Start();
            try
            {
                listener.BeginGetContext(GotCallback, null);
            }
            catch (HttpListenerException)
            {
                // this will be thrown when listener is closed while waiting for a request
                return;
            }

        }

        private void GotCallback(IAsyncResult ar)
        {
            try
            {
                HttpListenerContext ctx = listener.EndGetContext(ar);
                listener.BeginGetContext(GotCallback, null);
                Process(ctx);
            }
            catch (HttpListenerException)
            {
                // this will be thrown when listener is closed while waiting for a request
                return;
            }
        }

        private void Process(HttpListenerContext ctx)
        {
            var nancyRequest = ConvertRequestToNancyRequest(ctx.Request);
            using (var nancyContext = engine.HandleRequest(nancyRequest))
            {
                ConvertNancyResponseToResponse(nancyContext.Response, ctx.Response);
            }
        }

        public void Stop()
        {
            listener.Stop();
        }

    
        private static Uri GetUrlAndPathComponents(Uri uri) 
        {
            // ensures that for a given url only the
            //  scheme://host:port/paths/somepath
            return new Uri(uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped));
        }

        private Request ConvertRequestToNancyRequest(HttpListenerRequest request)
        {
            var baseUri = baseUriList.FirstOrDefault(uri => uri.IsBaseOf(request.Url));

            if (baseUri == null)
            {
                throw new InvalidOperationException(String.Format("Unable to locate base URI for request: {0}",request.Url));
            }

            var expectedRequestLength =
                GetExpectedRequestLength(request.Headers.ToDictionary());

            var relativeUrl =
                GetUrlAndPathComponents(baseUri).MakeRelativeUri(GetUrlAndPathComponents(request.Url));

            var nancyUrl = new Url
            {
                Scheme = request.Url.Scheme,
                HostName = request.Url.Host,
                Port = request.Url.IsDefaultPort ? null : (int?)request.Url.Port,
                BasePath = baseUri.AbsolutePath.TrimEnd('/'),
                Path = string.Concat("/", relativeUrl),
                Query = request.Url.Query,
                Fragment = request.Url.Fragment,
            };

            return new Request(
                request.HttpMethod,
                nancyUrl,
                RequestStream.FromStream(request.InputStream, expectedRequestLength, true),
                request.Headers.ToDictionary(), 
                (request.RemoteEndPoint != null) ? request.RemoteEndPoint.Address.ToString() : null);
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

        private static void ConvertNancyResponseToResponse(Response nancyResponse, HttpListenerResponse response)
        {
            foreach (var header in nancyResponse.Headers)
            {
                response.AddHeader(header.Key, header.Value);
            }

            foreach (var nancyCookie in nancyResponse.Cookies)
            {
                response.Cookies.Add(ConvertCookie(nancyCookie));
            }

            response.ContentType = nancyResponse.ContentType;
            response.StatusCode = (int)nancyResponse.StatusCode;

            using (var output = response.OutputStream)
            {
                nancyResponse.Contents.Invoke(output);
            }
        }

        private static Cookie ConvertCookie(INancyCookie nancyCookie)
        {
            var cookie = 
                new Cookie(nancyCookie.EncodedName, nancyCookie.EncodedValue, nancyCookie.Path, nancyCookie.Domain);

            if (nancyCookie.Expires.HasValue)
            {
                cookie.Expires = nancyCookie.Expires.Value;
            }

            return cookie;
        }
    }
}