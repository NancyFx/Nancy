namespace Nancy
{
    using System;
    using System.Linq;

    using Extensions;
    using Nancy.Responses;
    using System.IO;

    public static class FormatterExtensions
    {
        private static ISerializer jsonSerializer;

        private static ISerializer xmlSerializer;

        public static Response AsFile(this IResponseFormatter formatter, string applicationRelativeFilePath, string contentType)
        {
            return new GenericFileResponse(applicationRelativeFilePath, contentType);
        }

        public static Response AsFile(this IResponseFormatter formatter, string applicationRelativeFilePath)
        {
            return new GenericFileResponse(applicationRelativeFilePath);
        }

        public static Response AsCss(this IResponseFormatter formatter, string applicationRelativeFilePath)
        {
            return AsFile(formatter, applicationRelativeFilePath);
        }

        public static Response AsImage(this IResponseFormatter formatter, string applicationRelativeFilePath)
        {
            return AsFile(formatter, applicationRelativeFilePath);
        }

        public static Response AsJs(this IResponseFormatter formatter, string applicationRelativeFilePath)
        {
            return AsFile(formatter, applicationRelativeFilePath);
        }

        public static Response AsJson<TModel>(this IResponseFormatter formatter, TModel model, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var serializer = jsonSerializer ?? (jsonSerializer = formatter.Serializers.FirstOrDefault(s => s.CanSerialize("application/json")));

            var r = new JsonResponse<TModel>(model, serializer);
        	r.StatusCode = statusCode;

        	return r;
        }

        public static Response AsRedirect(this IResponseFormatter formatter, string location, Nancy.Responses.RedirectResponse.RedirectType type = RedirectResponse.RedirectType.SeeOther)
        {
            return new RedirectResponse(formatter.Context.ToFullPath(location), type);
        }

        public static Response AsXml<TModel>(this IResponseFormatter formatter, TModel model)
        {
            var serializer = xmlSerializer ?? (xmlSerializer = formatter.Serializers.FirstOrDefault(s => s.CanSerialize("application/xml")));

            return new XmlResponse<TModel>(model, "application/xml", serializer);
        }
        
        public static Response FromStream(this IResponseFormatter formatter, Stream stream, string contentType)
        {
            return new StreamResponse(() => stream, contentType);
        }

        public static Response FromStream(this IResponseFormatter formatter, Func<Stream> streamDelegate, string contentType)
        {
            return new StreamResponse(streamDelegate, contentType);
        }
    }
}