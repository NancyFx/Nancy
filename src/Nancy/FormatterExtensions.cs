namespace Nancy
{
    using System;
    using System.IO;

    using Nancy.Extensions;
    using Nancy.Responses;

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

        public static Response AsText(this IResponseFormatter formatter, string contents, string contentType)
        {
            return new TextResponse(contents, contentType);
        }

        public static Response AsText(this IResponseFormatter formatter, string contents)
        {
            return new TextResponse(contents);
        }

        public static Response AsImage(this IResponseFormatter formatter, string applicationRelativeFilePath)
        {
            return AsFile(formatter, applicationRelativeFilePath);
        }

        public static Response AsJson<TModel>(this IResponseFormatter formatter, TModel model, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var serializer = jsonSerializer ?? (jsonSerializer = formatter.SerializerFactory.GetSerializer("application/json"));

            return new JsonResponse<TModel>(model, serializer)
            {
                StatusCode = statusCode
            };
        }

        public static Response AsRedirect(this IResponseFormatter formatter, string location, RedirectResponse.RedirectType type = RedirectResponse.RedirectType.SeeOther)
        {
            return new RedirectResponse(formatter.Context.ToFullPath(location), type);
        }

        public static Response AsXml<TModel>(this IResponseFormatter formatter, TModel model)
        {
            var serializer = xmlSerializer ?? (xmlSerializer = formatter.SerializerFactory.GetSerializer("application/xml"));

            return new XmlResponse<TModel>(model, serializer);
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