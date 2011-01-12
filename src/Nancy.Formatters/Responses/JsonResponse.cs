using System.Runtime.Serialization.Json;
using System.Text;

namespace Nancy.Formatters.Responses
{
    using System;
    using System.IO;
    using System.Net;
    using Newtonsoft.Json;

    public interface IProvideJSonSerialization
    {
        Action<Stream> SerializeToJson(object model);

        TModel DeserializeFromJson<TModel>(dynamic jsonDataFromRequest);
    }

    /// <summary>
    /// Provides json serializing using the newtonsoft Json.net library
    /// Does not require objects to be decorated with DataContract attribute.
    /// </summary>
    public class NewtonsoftJsonSerializer : IProvideJSonSerialization
    {
        public Action<Stream> SerializeToJson(object model)
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

        public TModel DeserializeFromJson<TModel>(dynamic jsonDataFromRequest)
        {
            return JsonConvert.DeserializeObject<TModel>((string)jsonDataFromRequest);
        }
    }

    /// <summary>
    /// Provides Json serializing using
    /// DataContractJsonSerializer
    /// Note: Requires objects to be decorated with DataContract attribute 
    /// </summary>
    public class JsonSerializerUsingDataContractJsonSerializer : IProvideJSonSerialization
    {
        public Action<Stream> SerializeToJson(object model)
        {
            return stream =>
                {
                    var serializer = new DataContractJsonSerializer(model.GetType());
                    serializer.WriteObject(stream, model);
                };
        }

        public TModel DeserializeFromJson<TModel>(dynamic jsonDataFromRequest)
        {
            using (var stream = new MemoryStream(Encoding.Default.GetBytes((string)jsonDataFromRequest)))
            {
                var serializer = new DataContractJsonSerializer(typeof (TModel));
                return (TModel) serializer.ReadObject(stream);
            }
        }
    }

    public class JsonResponse<TModel> : Response
    {
        public JsonResponse(TModel model, IProvideJSonSerialization serializer)
        {
            this.Contents = serializer.SerializeToJson(model);
            this.ContentType = "application/json";
            this.StatusCode = HttpStatusCode.OK;
        }
        
    }
}