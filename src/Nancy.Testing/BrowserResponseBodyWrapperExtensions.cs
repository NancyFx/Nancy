namespace Nancy.Testing
{
    using System;
    using System.Xml;
    using Nancy.Json;

    /// <summary>
    /// Extension method for formatting the contents of a <see cref="BrowserResponseBodyWrapper"/>.
    /// </summary>
    public static class BrowserResponseBodyWrapperExtensions
    {
        /// <summary>
        /// Gets the HTTP response body wrapped in a string.
        /// </summary>
        /// <value>A string containing the HTTP response body.</value>
        public static TModel AsJson<TModel>(this BrowserResponseBodyWrapper bodyWrapper)
        {
            var serializer = 
                new JavaScriptSerializer();

            return serializer.Deserialize<TModel>(bodyWrapper.AsString());
        }

        /// <summary>
        /// Gets the HTTP response body wrapped in a string.
        /// </summary>
        /// <value>A string containing the HTTP response body.</value>
        public static string AsString(this BrowserResponseBodyWrapper bodyWrapper)
        {
            return Convert.ToString(bodyWrapper);
        }

        /// <summary>
        /// Gets the HTTP response body as a <see cref="XmlDocument"/>
        /// </summary>
        /// <value>A <see cref="XmlDocument"/> representation of the HTTP response body.</value>
        public static XmlDocument AsXml(this BrowserResponseBodyWrapper bodyWrapper)
        {
            var document =
                new XmlDocument();
            document.LoadXml(bodyWrapper.AsString());

            return document;
        }
    }
}