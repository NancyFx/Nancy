namespace Nancy.Hosting.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;
    using System.Xml;
    using IO;
    using Nancy.Bootstrapper;
    using Nancy.Extensions;

    /// <summary>
    /// Host for running Nancy ontop of WCF.
    /// </summary>
    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class NancyWcfGenericService
    {
        private readonly INancyEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyWcfGenericService"/> class with a default bootstrapper.
        /// </summary>
        public NancyWcfGenericService()
            : this(NancyBootstrapperLocator.Bootstrapper)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyWcfGenericService"/> class, with the provided <paramref name="bootstrapper"/>.
        /// </summary>
        /// <param name="bootstrapper">An <see cref="INancyBootstrapper"/> instance, that should be used to handle the requests.</param>
        public NancyWcfGenericService(INancyBootstrapper bootstrapper)
        {
            bootstrapper.Initialise();
            this.engine = bootstrapper.GetEngine();
        }
        
        /// <summary>
        /// Handels an incoming request with Nancy.
        /// </summary>
        /// <param name="requestBody">The body of the incoming request.</param>
        /// <returns>A <see cref="Message"/> instance.</returns>
        [WebInvoke(UriTemplate = "*", Method = "*")]
        public Message HandleRequests(Stream requestBody)
        {
            var webContext = WebOperationContext.Current;
            
            var nancyRequest = 
                CreateNancyRequestFromIncomingWebRequest(webContext.IncomingRequest, requestBody);

            var nancyContext = 
                engine.HandleRequest(nancyRequest);

            SetNancyResponseToOutgoingWebResponse(webContext.OutgoingResponse, nancyContext.Response);

            return webContext.CreateStreamResponse(
                stream =>
                    {
                        nancyContext.Response.Contents(stream);
                        nancyContext.Dispose();
                    }, 
                    nancyContext.Response.ContentType ?? "none/none"); // Stupid WCF forces us to specify a content type
        }

        private static Request CreateNancyRequestFromIncomingWebRequest(IncomingWebRequestContext webRequest, Stream requestBody)
        {
            var address =
                ((RemoteEndpointMessageProperty)
                 OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name]);

            var baseUri =
                GetUrlAndPathComponents(webRequest.UriTemplateMatch.BaseUri);

            if(!baseUri.OriginalString.EndsWith("/"))
            {
                baseUri = new Uri(string.Concat(baseUri.OriginalString, "/"));
            }

            var relativeUri =
                baseUri.MakeRelativeUri(GetUrlAndPathComponents(webRequest.UriTemplateMatch.RequestUri));

            var expectedRequestLength =
                GetExpectedRequestLength(webRequest.Headers.ToDictionary());

            var nancyUrl = new Url {
                BasePath = webRequest.UriTemplateMatch.BaseUri.AbsolutePath,
                Scheme = webRequest.UriTemplateMatch.RequestUri.Scheme,
                HostName = webRequest.UriTemplateMatch.BaseUri.Host,
                Port = webRequest.UriTemplateMatch.RequestUri.IsDefaultPort ? null : (int?)webRequest.UriTemplateMatch.RequestUri.Port,                    
                Path = string.Concat("/", relativeUri),
                Query = webRequest.UriTemplateMatch.RequestUri.Query
            };

            return new Request(
                webRequest.Method,
                nancyUrl,
                RequestStream.FromStream(requestBody, expectedRequestLength, false),
                webRequest.Headers.ToDictionary(),
                address.Address);
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

        private static Uri GetUrlAndPathComponents(Uri uri) 
        {
            // ensures that for a given url only the
            // scheme://host:port/paths/somepath
            return new Uri(uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped));
        }

        private static void SetNancyResponseToOutgoingWebResponse(OutgoingWebResponseContext webResponse, Response nancyResponse)
        {
            SetHttpResponseHeaders(webResponse, nancyResponse);

            if (nancyResponse.ContentType != null)
            {
                webResponse.ContentType = nancyResponse.ContentType;
            }
            webResponse.StatusCode = (System.Net.HttpStatusCode)nancyResponse.StatusCode;
        }

        private static void SetHttpResponseHeaders(OutgoingWebResponseContext context, Response response)
        {
            foreach (var kvp in response.Headers)
            {
                context.Headers.Add(kvp.Key, kvp.Value);
            }
            foreach (var cookie in response.Cookies)
            {
                context.Headers.Add("Set-Cookie", cookie.ToString());
            }
        }
    }
}
