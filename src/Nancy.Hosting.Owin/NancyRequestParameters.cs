namespace Nancy.Hosting.Owin
{
    using System.Collections.Generic;
    using Nancy.IO;

    /// <summary>
    /// Holds the values used to construct a Nancy <see cref="Request"/> instance.
    /// </summary>
    public class NancyRequestParameters
    {
        /// <summary>
        /// Gets a <see cref="RequestStream"/> that can be used to read the incoming HTTP body
        /// </summary>
        /// <value>A <see cref="RequestStream"/> object representing the incoming HTTP body.</value>
        public RequestStream Body { get; set; }

        /// <summary>
        /// Gets the HTTP headers sent by the client.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> containing the name and values of the headers.</value>
        /// <remarks>The values are stored in an <see cref="IEnumerable{T}"/> of string to be compliant with multi-value headers.</remarks>
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }

        /// <summary>
        /// Gets or sets the HTTP data transfer method used by the client.
        /// </summary>
        /// <value>The method.</value>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the HTTP protocol used by the client.
        /// </summary>
        /// <value>The protocol.</value>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets the querystring data of the requested resource.
        /// </summary>
        /// <value>A <see cref="DynamicDictionary"/>instance, containing the key/value pairs of querystring data.</value>
        public string Query { get; set; }

        /// <summary>
        /// Gets the absolute path of the requested resource. 
        /// </summary>
        /// <value>A <see cref="string"/> containing the absolute path of the requested resource.</value>
        /// <remarks>This does not include the scheme, host name, or query portion of the URI.</remarks>
        public string Uri { get; set; }
    }
}