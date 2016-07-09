namespace Nancy.Testing
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Nancy.IO;

    /// <summary>
    /// Wrapper for the HTTP response body that is used by the <see cref="BrowserResponse"/> class.
    /// </summary>
    public class BrowserResponseBodyWrapper : IEnumerable<byte>
    {
        private readonly IEnumerable<byte> responseBytes;
        private readonly string contentType;
        private DocumentWrapper responseDocument;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserResponseBodyWrapper"/> class.
        /// </summary>
        /// <param name="response">The <see cref="Response"/> to wrap.</param>
        /// <param name="browserContext">The <see cref="BrowserContext"/> of the request that generated the response.</param>
        public BrowserResponseBodyWrapper(Response response, BrowserContext browserContext)
        {
            this.BrowserContext = browserContext;
            var contentStream = GetContentStream(response);

            this.responseBytes = contentStream.ToArray();
            this.contentType = response.ContentType;
        }

        /// <summary>
        /// Gets the content type of the wrapped response.
        /// </summary>
        /// <returns>A string containing the content type.</returns>
        public string ContentType
        {
            get { return this.contentType; }
        }

        /// <summary>
        /// Gets the <see cref="BrowserContext"/> of the request that generated the response.
        /// </summary>
        /// <value>A <see cref="BrowserContext"/> intance.</value>
        public BrowserContext BrowserContext { get; private set; }

        private static MemoryStream GetContentStream(Response response)
        {
            var contentsStream = new MemoryStream();

            var unclosableStream = new UnclosableStreamWrapper(contentsStream);

            response.Contents.Invoke(unclosableStream);
            contentsStream.Position = 0;

            return contentsStream;
        }

        /// <summary>
        /// Gets a <see cref="QueryWrapper"/> for the provided <paramref name="selector"/>.
        /// </summary>
        /// <param name="selector">The CSS3 selector that should be applied.</param>
        /// <returns>A <see cref="QueryWrapper"/> instance.</returns>
        public QueryWrapper this[string selector]
        {
            get
            {
                if (this.responseDocument == null)
                {
                    this.responseDocument = new DocumentWrapper(this.responseBytes);
                }

                return this.responseDocument[selector];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<byte> GetEnumerator()
        {
            return this.responseBytes.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
