namespace Nancy.Responses
{
    using System;
    using System.IO;
    using Nancy.Configuration;
    using Nancy.Json;

    /// <summary>
    /// Represents a JSON response of the type <typeparamref name="TModel"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public class JsonResponse<TModel> : Response
    {
        private readonly JsonConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse{TModel}"/> class,
        /// with the provided <paramref name="model"/>, <paramref name="serializer"/>
        /// and <paramref name="environment"/>.
        /// </summary>
        /// <param name="model">The model that should be returned as JSON.</param>
        /// <param name="serializer">The <see cref="ISerializer"/> to use for the serialization.</param>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public JsonResponse(TModel model, ISerializer serializer, INancyEnvironment environment)
        {
            if (serializer == null)
            {
                throw new InvalidOperationException("JSON Serializer not set");
            }

            this.configuration = environment.GetValue<JsonConfiguration>();
            this.Contents = model == null ? NoBody : this.GetJsonContents(model, serializer);
            this.ContentType = this.DefaultContentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private string DefaultContentType
        {
            get { return string.Concat("application/json", this.Encoding); }
        }

        private string Encoding
        {
            get { return string.Concat("; charset=", this.configuration.DefaultEncoding.WebName); }
        }

        private Action<Stream> GetJsonContents(TModel model, ISerializer serializer)
        {
            return stream => serializer.Serialize(this.DefaultContentType, model, stream);
        }
    }

    /// <summary>
    /// Represents a JSON response
    /// </summary>
    public class JsonResponse : JsonResponse<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse{TModel}"/> class,
        /// with the provided <paramref name="model"/>, <paramref name="serializer"/>
        /// and <paramref name="environment"/>.
        /// </summary>
        /// <param name="model">The model that should be returned as JSON.</param>
        /// <param name="serializer">The <see cref="ISerializer"/> to use for the serialization.</param>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public JsonResponse(object model, ISerializer serializer, INancyEnvironment environment) : base(model, serializer, environment)
        {
        }
    }
}
