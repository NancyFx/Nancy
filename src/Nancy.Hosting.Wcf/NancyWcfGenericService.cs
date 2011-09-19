namespace Nancy.Hosting.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;
    using IO;
    using Nancy.Bootstrapper;
    using Nancy.Extensions;

    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
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

        public NancyWcfGenericService(INancyBootstrapper bootstrapper)
        {
            bootstrapper.Initialise();
            engine = bootstrapper.GetEngine();
        }
        
        [WebInvoke(UriTemplate = "*", Method = "*")]
        public Message HandleRequests(Stream requestBody)
        {
            var webContext = WebOperationContext.Current;
            
            var nancyRequest = CreateNancyRequestFromIncomingWebRequest(webContext.IncomingRequest, requestBody);
            var nancyContext = engine.HandleRequest(nancyRequest);

            SetNancyResponseToOutgoingWebResponse(webContext.OutgoingResponse, nancyContext.Response);
            
            return webContext.CreateStreamResponse(nancyContext.Response.Contents, nancyContext.Response.ContentType);
        }

        private static Uri GetUrlAndPathComponents(Uri uri) 
        {
            // ensures that for a given url only the
            //  scheme://host:port/paths/somepath
            return new Uri(uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped));
        }

        private static Request CreateNancyRequestFromIncomingWebRequest(IncomingWebRequestContext webRequest, Stream requestBody)
        {
            var address =
                ((RemoteEndpointMessageProperty)
                 OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name]);
            var relativeUri = GetUrlAndPathComponents(webRequest.UriTemplateMatch.BaseUri).MakeRelativeUri(GetUrlAndPathComponents(webRequest.UriTemplateMatch.RequestUri));

            var expectedRequestLength =
                GetExpectedRequestLength(webRequest.Headers.ToDictionary());

            return new Request(
                webRequest.Method,
                string.Concat("/", relativeUri),
                webRequest.Headers.ToDictionary(),
                RequestStream.FromStream(requestBody, expectedRequestLength, false),
                webRequest.UriTemplateMatch.RequestUri.Scheme,
                webRequest.UriTemplateMatch.RequestUri.Query,
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
            if (!long.TryParse(headerValue, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength))
            {
                return 0;
            }

            return contentLength;
        }

        private static void SetNancyResponseToOutgoingWebResponse(OutgoingWebResponseContext webResponse, Response nancyResponse)
        {
            SetHttpResponseHeaders(webResponse, nancyResponse);

            webResponse.ContentType = nancyResponse.ContentType;
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
