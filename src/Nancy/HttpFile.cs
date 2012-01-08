namespace Nancy
{
    using System.IO;

    /// <summary>
    /// Represents a file that was captures in a HTTP multipart/form-data request
    /// </summary>
    public class HttpFile
    {
        public HttpFile(string contentType, string name, Stream value, string key)
        {
            this.ContentType = contentType;
            this.Name = name;
            this.Value = value;
            this.Key = key;
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>A <see cref="string"/> containing the content type of the file.</value>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the file.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the form element name of this file
        /// </summary>
        /// <value>A <see cref="string"/> containg the key</value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets the value stream.
        /// </summary>
        /// <value>A <see cref="Stream"/> containing the contents of the file.</value>
        /// <remarks>This is a <see cref="HttpMultiparSubStream"/> instance that sits ontop of the request stream.</remarks>
        public Stream Value { get; private set; }
    }
}