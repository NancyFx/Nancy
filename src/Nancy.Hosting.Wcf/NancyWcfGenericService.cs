namespace Nancy.Hosting.Wcf
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;
    using Nancy.Extensions;
    using Nancy.Routing;

    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class NancyWcfGenericService
    {
        private readonly NancyEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyWcfGenericService"/> class.
        /// </summary>
        public NancyWcfGenericService()
            : this(new AppDomainModuleLocator(new DefaultModuleActivator()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyWcfGenericService"/> class.
        /// </summary>
        /// <param name="moduleLocator">An <see cref="INancyModuleLocator"/> instance that will be used by Nancy to decect available modules.</param>
        public NancyWcfGenericService(INancyModuleLocator moduleLocator)
        {
            engine = new NancyEngine(moduleLocator, new RouteResolver());
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
                requestBody);
        }

        private static void SetNancyResponseToOutgoingWebResponse(OutgoingWebResponseContext webResponse, Response nancyResponse)
        {
            webResponse.ContentType = nancyResponse.ContentType;
            webResponse.StatusCode = nancyResponse.StatusCode;
        }
    }
}