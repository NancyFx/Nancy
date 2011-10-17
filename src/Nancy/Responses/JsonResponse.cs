namespace Nancy.Responses
{
    using System;
    using System.IO;
    using Json;

    public class JsonResponse<TModel> : Response
    {
        public JsonResponse(TModel model)
        {
            if (DefaultSerializersStartup.JsonSerializer == null)
            {
                throw new InvalidOperationException("JSON Serializer not set");
            }

            this.Contents = GetJsonContents(model);
            this.ContentType = "application/json";
            this.StatusCode = HttpStatusCode.OK;
        }
     
        private static Action<Stream> GetJsonContents(TModel model)
        {
            return stream => DefaultSerializersStartup.JsonSerializer.Serialize("application/json", model, stream);
        }
    }

    public class JsonResponse : JsonResponse<object>
    {
        public JsonResponse(object model) : base(model)
        {
        }
    }
}