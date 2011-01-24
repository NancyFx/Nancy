namespace Nancy.Hosting.SelfHosting
{
    using System;
    using System.Net;
    using System.Threading;
    using BootStrapper;
    using Cookies;
    using Extensions;


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
        private readonly Uri _baseUri;
        private readonly HttpListener _listener;
        private readonly INancyEngine _engine;
        private Thread _thread;
        private bool _shouldContinue;

        public NancyHost(Uri baseUri)
            : this(baseUri, NancyBootStrapperLocator.BootStrapper)
        {
        }

        public NancyHost(Uri baseUri, INancyBootStrapper bootStrapper)
        {
            _baseUri = baseUri;
            _listener = new HttpListener();
            _listener.Prefixes.Add(baseUri.ToString());

            _engine = bootStrapper.GetEngine();
        }

        public void Start()
        {
            _shouldContinue = true;

            _listener.Start();
            _thread = new Thread(Listen);
            _thread.Start();
        }

        public void Stop()
        {
            _shouldContinue = false;
            _listener.Stop();
        }

        private void Listen()
        {
            while (_shouldContinue)
            {
                HttpListenerContext requestContext;
                try
                {
                    requestContext = _listener.GetContext();
                }
                catch (HttpListenerException)
                {
                    // this will be thrown when listener is closed while waiting for a request
                    return;
                }
                var nancyRequest = ConvertRequestToNancyRequest(requestContext.Request);
                var nancyResponse = _engine.HandleRequest(nancyRequest);
                ConvertNancyResponseToResponse(nancyResponse, requestContext.Response);
            }
        }

        private IRequest ConvertRequestToNancyRequest(HttpListenerRequest request)
        {
            var relativeUrl = "/" + _baseUri.MakeRelativeUri(request.Url);

            return new Request(
                request.HttpMethod,
                relativeUrl,
                request.Headers.ToDictionary(),
                request.InputStream,
                request.Url.Scheme
                );
        }

        private static void ConvertNancyResponseToResponse(Response nancyResponse, HttpListenerResponse response)
        {
            foreach (var header in nancyResponse.Headers)
                response.AddHeader(header.Key, header.Value);
            foreach (var nancyCookie in nancyResponse.Cookies)
                response.Cookies.Add(ConvertCookie(nancyCookie));

            response.ContentType = nancyResponse.ContentType;

            using (var output = response.OutputStream)
                nancyResponse.Contents(output);
        }

        private static Cookie ConvertCookie(INancyCookie nancyCookie)
        {
            var cookie = new Cookie(nancyCookie.Name, nancyCookie.Value, nancyCookie.Path, nancyCookie.Domain);
            if (nancyCookie.Expires.HasValue)
                cookie.Expires = nancyCookie.Expires.Value;
            return cookie;
        }
    }
}