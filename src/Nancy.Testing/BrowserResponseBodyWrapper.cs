namespace Nancy.Testing
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Wrapper for the HTTP response body that is used by the <see cref="BrowserResponse"/> class.
    /// </summary>
    public class BrowserResponseBodyWrapper : IEnumerable<byte>
    {
        private readonly IEnumerable<byte> responseBytes;
        private readonly string contentType;
        private DocumentWrapper responseDocument;

        public BrowserResponseBodyWrapper(Response response)
        {
            var contentStream = GetContentStream(response);

            this.responseBytes = contentStream.ToArray();
            this.contentType = response.ContentType;
        }

        internal string ContentType
        {
            get { return this.contentType; }
        }

        private static MemoryStream GetContentStream(Response response)
        {
            var contentsStream = new MemoryStream();
            response.Contents.Invoke(contentsStream);
            contentsStream.Position = 0;
            return contentsStream;
        }

        /// <summary>
        /// Gets a <see cref="QueryWrapper"/> for the provided <paramref name="selector"/>.
        /// </summary>
        /// <param name="selector">The CSS3 that shuold be applied.</param>
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