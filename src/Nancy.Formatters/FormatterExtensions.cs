using System;
using System.IO;

namespace Nancy.Formatters
{
    using Responses;

    public static class FormatterExtensions
    {
        public static Response AsJson<TModel>(this IResponseFormatter formatter, TModel model)
        {
            return new JsonResponse<TModel>(model, new NewtonsoftJsonSerializer());
        }

        public static Response AsJson<TModel>(this IResponseFormatter fornatter, TModel model, IProvideJSonSerialization serializer)
        {
            return new JsonResponse<TModel>(model, serializer);
        }

        public static Response AsXml<TModel>(this IResponseFormatter formatter, TModel model)
        {
            return new XmlResponse<TModel>(model);
        }

        public static Response Image(this IResponseFormatter formatter, string imagePath)
        {
            return new ImageResponse(imagePath);
        }
    }
}