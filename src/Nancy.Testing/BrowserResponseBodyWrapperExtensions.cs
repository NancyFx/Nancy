namespace Nancy.Testing
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Nancy.Json;

    /// <summary>
    /// Extension method for formatting the contents of a <see cref="BrowserResponseBodyWrapper"/>.
    /// </summary>
    public static class BrowserResponseBodyWrapperExtensions
    {
        /// <summary>
        /// Gets the HTTP response body wrapped in a <see cref="Stream"/>.
        /// </summary>
        /// <param name="bodyWrapper">An instance of the <see cref="BrowserResponseBodyWrapper"/> that the extension should be invoked on.</param>
        /// <returns>A <see cref="Stream"/> representation of the HTTP response body.</returns>
        public static Stream AsStream(this BrowserResponseBodyWrapper bodyWrapper)
        {
            return new MemoryStream(bodyWrapper.ToArray());
        }

        /// <summary>
        /// Gets the HTTP response body wrapped in a string.
        /// </summary>
        /// <param name="bodyWrapper">An instance of the <see cref="BrowserResponseBodyWrapper"/> that the extension should be invoked on.</param>
        /// <value>A string containing the HTTP response body.</value>
        public static string AsString(this BrowserResponseBodyWrapper bodyWrapper)
        {
            return Encoding.UTF8.GetString(bodyWrapper.ToArray());
        }

        /// <summary>
        /// Gets the HTTP response body as a <see cref="XmlDocument"/>
        /// </summary>
        /// <param name="bodyWrapper">An instance of the <see cref="BrowserResponseBodyWrapper"/> that the extension should be invoked on.</param>
        /// <value>A <see cref="XmlDocument"/> representation of the HTTP response body.</value>
        public static XmlDocument AsXmlDocument(this BrowserResponseBodyWrapper bodyWrapper)
        {
            var document =
                new XmlDocument();
            document.LoadXml(bodyWrapper.AsString());

            return document;
        }

        /// <summary>
        /// Gets the deserialized representation of the JSON in the response body.
        /// </summary>
        /// <typeparam name="TModel">The type that the JSON response body should be deserialized to.</typeparam>
        /// <param name="bodyWrapper">An instance of the <see cref="BrowserResponseBodyWrapper"/> that the extension should be invoked on.</param>
        /// <value>A <typeparamref name="TModel"/> instance representation of the HTTP response body.</value>
        public static TModel DeserializeJson<TModel>(this BrowserResponseBodyWrapper bodyWrapper)
        {
            var serializer = 
                new JavaScriptSerializer();

            return bodyWrapper.DeserializeJson<TModel>(serializer);
        }

        /// <summary>
        /// Gets the deserialized representation of the JSON in the response body.
        /// </summary>
        /// <typeparam name="TModel">The type that the JSON response body should be deserialized to.</typeparam>
        /// <param name="bodyWrapper">An instance of the <see cref="BrowserResponseBodyWrapper"/> that the extension should be invoked on.</param>
        /// <param name="serializer">An instance of <see cref="JavaScriptSerializer"/>, controlling the deserialization.</param>
        /// <value>A <typeparamref name="TModel"/> instance representation of the HTTP response body.</value>
        public static TModel DeserializeJson<TModel>(this BrowserResponseBodyWrapper bodyWrapper, JavaScriptSerializer serializer)
        {
            return serializer.Deserialize<TModel>(bodyWrapper.AsString());
        }

        /// <summary>
        /// Gets the deserialized representation of the XML in the response body.
        /// </summary>
        /// <typeparam name="TModel">The type that the XML response body should be deserialized to.</typeparam>
        /// <param name="bodyWrapper">An instance of the <see cref="BrowserResponseBodyWrapper"/> that the extension should be invoked on.</param>
        /// <value>A <typeparamref name="TModel"/> instance representation of the HTTP response body.</value>
        public static TModel DeserializeXml<TModel>(this BrowserResponseBodyWrapper bodyWrapper)
        {
            var serializer =
                new XmlSerializer(typeof(TModel));

            return (TModel)serializer.Deserialize(bodyWrapper.AsStream());
        }
    }
}
