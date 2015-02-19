namespace Nancy.Hosting.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Claims;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;

    using Nancy.Bootstrapper;
    using Nancy.Extensions;
    using Nancy.IO;

    /// <summary>
    /// Host for running Nancy ontop of WCF.
    /// </summary>
    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class NancyWcfGenericService : IDisposable
    {
        private readonly INancyEngine engine;
        private readonly INancyBootstrapper bootstrapper;


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
            this.bootstrapper = bootstrapper;
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
                CreateNancyRequestFromIncomingWebRequest(webContext.IncomingRequest, requestBody, OperationContext.Current);

            var nancyContext = 
                engine.HandleRequest(nancyRequest);

            SetNancyResponseToOutgoingWebResponse(webContext.OutgoingResponse, nancyContext.Response);

            return webContext.CreateStreamResponse(
                stream =>
                    {
                        nancyContext.Response.Contents(stream);
                        nancyContext.Dispose();
                    }, 
                    nancyContext.Response.ContentType ?? string.Empty);
        }

        public void Dispose()
        {
            this.bootstrapper.Dispose();
        }

        private static Request CreateNancyRequestFromIncomingWebRequest(IncomingWebRequestContext webRequest, Stream requestBody, OperationContext context)
        {
            string ip = null;

            object remoteEndpointProperty;
            if (OperationContext.Current.IncomingMessageProperties.TryGetValue(RemoteEndpointMessageProperty.Name, out remoteEndpointProperty))
            {
                ip = ((RemoteEndpointMessageProperty)remoteEndpointProperty).Address;
            }

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

            byte[] certificate = null;

            if (context.ServiceSecurityContext != null && context.ServiceSecurityContext.AuthorizationContext.ClaimSets.Count > 0)
            {
                var claimset =
                    context.ServiceSecurityContext.AuthorizationContext.ClaimSets.FirstOrDefault(
                        c => c is X509CertificateClaimSet) as X509CertificateClaimSet;

                if (claimset != null)
                {
                    certificate = claimset.X509Certificate.RawData;
                }
            }

            return new Request(
                webRequest.Method,
                nancyUrl,
                RequestStream.FromStream(requestBody, expectedRequestLength, StaticConfiguration.DisableRequestStreamSwitching ?? false),
                webRequest.Headers.ToDictionary(),
                ip, 
                certificate);
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

            if (nancyResponse.ReasonPhrase != null)
            {
                webResponse.StatusDescription = nancyResponse.ReasonPhrase;
            }

            webResponse.StatusCode = (HttpStatusCode)nancyResponse.StatusCode;
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
