namespace Nancy.Hosting.SelfHosting
{
    using System;
    using System.Net;
    using System.Threading;
    using Bootstrapper;
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
        private readonly Uri baseUri;
        private readonly HttpListener listener;
        private readonly INancyEngine engine;
        private Thread thread;
        private bool shouldContinue;

        public NancyHost(Uri baseUri)
            : this(baseUri, NancyBootstrapperLocator.Bootstrapper)
        {
        }

        public NancyHost(Uri baseUri, INancyBootstrapper bootStrapper)
        {
            this.baseUri = baseUri;
            listener = new HttpListener();
            listener.Prefixes.Add(baseUri.ToString());

            engine = bootStrapper.GetEngine();
        }

        public void Start()
        {
            shouldContinue = true;

            listener.Start();
            thread = new Thread(Listen);
            thread.Start();
        }

        public void Stop()
        {
            shouldContinue = false;
            listener.Stop();
        }

        private void Listen()
        {
            while (shouldContinue)
            {
                HttpListenerContext requestContext;
                try
                {
                    requestContext = listener.GetContext();
                }
                catch (HttpListenerException)
                {
                    // this will be thrown when listener is closed while waiting for a request
                    return;
                }
                var nancyRequest = ConvertRequestToNancyRequest(requestContext.Request);
                var nancyResponse = engine.HandleRequest(nancyRequest);
                ConvertNancyResponseToResponse(nancyResponse, requestContext.Response);
            }
        }

        private Request ConvertRequestToNancyRequest(HttpListenerRequest request)
        {
            var relativeUrl = "/" + baseUri.MakeRelativeUri(request.Url);

            return new Request(
                request.HttpMethod,
                relativeUrl,
                request.Headers.ToDictionary(),
                request.InputStream,
                request.Url.Scheme,
                request.Url.Query);
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
                new Cookie(nancyCookie.Name, nancyCookie.Value, nancyCookie.Path, nancyCookie.Domain);

            if (nancyCookie.Expires.HasValue)
            {
                cookie.Expires = nancyCookie.Expires.Value;
            }

            return cookie;
        }
    }
}