namespace Nancy.Responses
{
    using System;
    using System.IO;
    using Json;

    public class JsonResponse<TModel> : Response
    {
        public JsonResponse(TModel model, ISerializer serializer)
        {
            if (serializer == null)
            {
                throw new InvalidOperationException("JSON Serializer not set");
            }

            this.Contents = GetJsonContents(model, serializer);
            this.ContentType = "application/json";
            this.StatusCode = HttpStatusCode.OK;
        }
     
        private static Action<Stream> GetJsonContents(TModel model, ISerializer serializer)
        {
            return stream => serializer.Serialize("application/json", model, stream);
        }
    }

    public class JsonResponse : JsonResponse<object>
    {
        public JsonResponse(object model, ISerializer serializer) : base(model, serializer)
        {
        }
    }
}