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

        public static Response AsJson<TModel>(this IResponseFormatter formatter, TModel model)
        {
            var serializer = jsonSerializer ?? (jsonSerializer = formatter.Serializers.FirstOrDefault(s => s.CanSerialize("application/json")));

            return new JsonResponse<TModel>(model, serializer);
        }

        public static Response AsRedirect(this IResponseFormatter formatter, string location)
        {
            return new RedirectResponse(formatter.Context.ToFullPath(location));
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