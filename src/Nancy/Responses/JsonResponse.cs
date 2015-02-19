namespace Nancy.Responses
{
    using System;
    using System.IO;

    using Nancy.Json;

    public class JsonResponse<TModel> : Response
    {
        public JsonResponse(TModel model, ISerializer serializer)
        {
            if (serializer == null)
            {
                throw new InvalidOperationException("JSON Serializer not set");
            }

            this.Contents = model == null ? NoBody : GetJsonContents(model, serializer);
            this.ContentType = DefaultContentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private static string DefaultContentType
        {
            get { return string.Concat("application/json", Encoding); }
        }

        private static string Encoding
        {
            get { return string.Concat("; charset=", JsonSettings.DefaultEncoding.WebName); }
        }

        private static Action<Stream> GetJsonContents(TModel model, ISerializer serializer)
        {
            return stream => serializer.Serialize(DefaultContentType, model, stream);
        }
    }

    public class JsonResponse : JsonResponse<object>
    {
        public JsonResponse(object model, ISerializer serializer) : base(model, serializer)
        {
        }
    }
}