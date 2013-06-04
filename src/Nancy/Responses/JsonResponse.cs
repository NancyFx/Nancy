namespace Nancy.Responses
{
    using System;
    using System.IO;
    using Json;

    public class JsonResponse<TModel> : Response
    {
        private static string _contentType = "application/json";
        /// <summary>
        /// Sets the char set.
        /// </summary>
        /// <value>The char set.</value>
        internal static string CharSet 
		{ 
            set 
			{
                _contentType = "application/json" + (String.IsNullOrWhiteSpace (value) ? "" : "; charset=" + value);
            }
        }

        public JsonResponse (TModel model, ISerializer serializer)
        {
            if (serializer == null) 
			{
                throw new InvalidOperationException ("JSON Serializer not set");
            }

            this.Contents = GetJsonContents (model, serializer);
            this.ContentType = _contentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetJsonContents (TModel model, ISerializer serializer)
        {
            return stream => serializer.Serialize (_contentType, model, stream);
        }
    }

    public class JsonResponse : JsonResponse<object>
    {
        public JsonResponse (object model, ISerializer serializer) : base(model, serializer)
        {
        }
    }
}