namespace Nancy.Formatters.Responses
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    public class JsonResponse<TModel> : Response
    {
        public JsonResponse(TModel model)
        {
            this.Contents = GetJsonContents(model);
            this.ContentType = "application/json";
        }
     
        private static Action<Stream> GetJsonContents(TModel model)
        {
            return stream =>
                       {
                           var serializer = new JsonSerializer();

                           //The caller will close the stream (needed for tests), do not dispose these writers here as this will close the stream 
                           var sw = new StreamWriter(stream);
                           var writer = new JsonTextWriter(sw);
                           
                           serializer.Serialize(writer, model);
                           
                           writer.Flush();
                           sw.Flush();
                       };
        }
    }
}