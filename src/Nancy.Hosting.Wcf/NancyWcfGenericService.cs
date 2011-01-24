namespace Nancy.Hosting.Wcf
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;
    using Nancy.BootStrapper;
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
            : this(NancyBootStrapperLocator.BootStrapper)
        {
        }

        public NancyWcfGenericService(INancyBootStrapper bootstrapper)
        {
            engine = bootstrapper.GetEngine();
        }
        
        [WebInvoke(UriTemplate = "*", Method = "*")]
        public Message HandleRequests(Stream requestBody)
        {
            var webContext = WebOperationContext.Current;

            var nancyRequest = CreateNancyRequestFromIncomingWebRequest(webContext.IncomingRequest, requestBody);
            var nancyResponse = engine.HandleRequest(nancyRequest);

            SetNancyResponseToOutgoingWebResponse(webContext.OutgoingResponse, nancyResponse);
            
            return webContext.CreateStreamResponse(nancyResponse.Contents, nancyResponse.ContentType);
        }

        private static IRequest CreateNancyRequestFromIncomingWebRequest(IncomingWebRequestContext webRequest, Stream requestBody)
        {
            var relativeUri =
                webRequest.UriTemplateMatch.BaseUri.MakeRelativeUri(webRequest.UriTemplateMatch.RequestUri);

            return new Request(
                webRequest.Method,
                string.Concat("/", relativeUri),
                webRequest.Headers.ToDictionary(),
                requestBody,
                webRequest.UriTemplateMatch.BaseUri.Scheme);
        }

        private static void SetNancyResponseToOutgoingWebResponse(OutgoingWebResponseContext webResponse, Response nancyResponse)
        {
            SetHttpResponseHeaders(webResponse, nancyResponse);

            webResponse.ContentType = nancyResponse.ContentType;
            webResponse.StatusCode = nancyResponse.StatusCode;
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
