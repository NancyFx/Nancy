namespace Nancy
{
    using System.IO;

    /// <summary>
    /// Represents a file that was captured in a HTTP multipart/form-data request
    /// </summary>
    public class HttpFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpFile"/> class,
        /// using the provided <paramref name="boundary"/>.
        /// </summary>
        /// <param name="boundary">The <see cref="HttpMultipartBoundary"/> that contains the file information.</param>
        public HttpFile(HttpMultipartBoundary boundary)
            : this(boundary.ContentType, boundary.Filename, boundary.Value, boundary.Name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpFile"/> class,
        /// using the provided values
        /// </summary>
        /// <paramref name="contentType">The content type of the file.</paramref>
        /// <paramref name="name">The name of the file.</paramref>
        /// <paramref name="value">The content of the file.</paramref>
        /// <paramref name="key">The name of the field that uploaded the file.</paramref>
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
        /// Gets or sets the form element name of this file.
        /// </summary>
        /// <value>A <see cref="string"/> containing the key.</value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets the value stream.
        /// </summary>
        /// <value>A <see cref="Stream"/> containing the contents of the file.</value>
        /// <remarks>This is a <see cref="HttpMultipartSubStream"/> instance that sits ontop of the request stream.</remarks>
        public Stream Value { get; private set; }
    }
}