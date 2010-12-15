using System;
using System.IO;
using Newtonsoft.Json;

namespace Nancy.Formatters
{
    public static class JsonFormatterExtensions
    {
        public static Action<Stream> Json<TModel>(this IResponseFormatter response, TModel model)
        {
            return stream =>
                       {
                           var serializer = new JsonSerializer();

                           using (var sw = new StreamWriter(stream))
                           using (JsonWriter writer = new JsonTextWriter(sw))
                           {
                               serializer.Serialize(writer, model);
                           }
                       };
        }
    }
}