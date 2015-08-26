namespace Nancy.ModelBinding.DefaultBodyDeserializers
{
    using Nancy.Json;

    /// <summary>
    /// Deserializes request bodies in JSON format while retaining their casing
    /// </summary>
    public class RetainCasingJsonBodyDeserializer : JsonBodyDeserializer
    {
        protected override JavaScriptSerializer GetSerializer()
        {
            var serializer = new JavaScriptSerializer(null,
                                                      false,
                                                      JsonSettings.MaxJsonLength,
                                                      JsonSettings.MaxRecursions,
                                                      true,
                                                      JsonSettings.ISO8601DateFormat);
            return serializer;
        }
    }
}
