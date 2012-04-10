namespace Nancy.Responses
{
    using System;
    using System.IO;

    /// <summary>
    /// Response that returns the contents of a stream of a given content-type.
    /// </summary>
    public class StreamResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamResponse"/> class with the
        /// provided stream provider and content-type.
        /// </summary>
        /// <param name="source">The value producer for the response.</param>
        /// <param name="contentType">The content-type of the stream contents.</param>
        /// <param name="lastModified">The last-modified date of the stream contents.</param>
        /// <param name="eTag">The entity tag cache validation string.</param>
        public StreamResponse(Func<Stream> source, string contentType, DateTime lastModified, string eTag)
        {
            if (lastModified.Year > 1753)
            {
                this.Headers["Last-Modified"] = lastModified.ToString("R");
            }
            if (!string.IsNullOrEmpty(eTag))
            {
                this.Headers["ETag"] = eTag;
            }
            this.Contents = GetResponseBodyDelegate(source);
            this.ContentType = contentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetResponseBodyDelegate(Func<Stream> sourceDelegate)
        {
            return stream =>
            {
                using (var source = sourceDelegate.Invoke())
                {
                    if (source.CanSeek)
                    {
                        source.Position = 0;
                    }

                    if (source.CanRead)
                    {
                        source.CopyTo(stream);
                    }
                }
            };
        }
    }
}
