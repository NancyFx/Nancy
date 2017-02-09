namespace Nancy.Extensions
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// Extensions for Stream.
    /// </summary>
    public static class StreamExtensions
    {
        internal const int BufferSize = 4096;

        /// <summary>
        /// Gets the request body as a string.
        /// </summary>
        /// <param name="stream">The request body stream.</param>
        /// <param name="encoding">The encoding to use, <see cref="Encoding.UTF8"/> by default.</param>
        /// <returns>The request body as a <see cref="string"/>.</returns>
        public static string AsString(this Stream stream, Encoding encoding = null)
        {
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8, true, BufferSize, true))
            {
                if (stream.CanSeek)
                {
                    var initialPosition = stream.Position;

                    stream.Position = 0;

                    var content = reader.ReadToEnd();

                    stream.Position = initialPosition;

                    return content;
                }
                return string.Empty;
            }
        }
    }
}
