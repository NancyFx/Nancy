namespace Nancy.Hosting.Wcf
{
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;
    using Nancy;
    using Nancy.Extensions;
    using Nancy.Routing;

    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class NancyWcfGenericService
    {
        private readonly NancyEngine engine;

        public NancyWcfGenericService(Assembly modulesAssembly)
        {
            this.engine = new NancyEngine(new NancyModuleLocator(modulesAssembly), new RouteResolver());
        }

        [WebInvoke(UriTemplate="*")]
        public Message HandleOther()
        {
            return HandleAll();
        }

        [WebGet(UriTemplate="*")]
        public Message HandleGet()
        {
            return HandleAll();
        }

        private Message HandleAll()
        {
            var context = WebOperationContext.Current;
            var request = CreateNancyRequestFromIncomingRequest(context.IncomingRequest);
            var response = this.engine.HandleRequest(request);
            
            SetNancyResponseToOutgoingResponse(context.OutgoingResponse, response);

            return context.CreateStreamResponse(response.Contents, response.ContentType);
        }

        private static IRequest CreateNancyRequestFromIncomingRequest(IncomingWebRequestContext request)
        {
            var relativeUri = 
                request.UriTemplateMatch.BaseUri.MakeRelativeUri(request.UriTemplateMatch.RequestUri);

            return new Request(
                request.Method,
                string.Concat("/", relativeUri),
                request.Headers.ToDictionary(),
                new MemoryStream());
        }

        private static void SetNancyResponseToOutgoingResponse(OutgoingWebResponseContext resp, Response response)
        {
            resp.ContentType = response.ContentType;
            resp.StatusCode = response.StatusCode;
        }
    }
}
