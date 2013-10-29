namespace Nancy
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents the content boundary of a HTTP multipart/form-data boundary in a stream.
    /// </summary>
    public class HttpMultipartBoundary
    {
        private const byte LF = (byte)'\n';
        private const byte CR = (byte)'\r';

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMultipartBoundary"/> class.
        /// </summary>
        /// <param name="boundaryStream">The stream that contains the boundary information.</param>
        public HttpMultipartBoundary(HttpMultipartSubStream boundaryStream)
        {
            this.Value = boundaryStream;
            this.ExtractHeaders();
        }

        /// <summary>
        /// Gets the contents type of the boundary value.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets or the filename for the boundary value.
        /// </summary>
        /// <value>A <see cref="string"/> containing the filename value if it is available; otherwise <see cref="string.Empty"/>.</value>
        /// <remarks>This is the RFC2047 decoded value of the filename attribute of the Content-Disposition header.</remarks>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets name of the boundary value.
        /// </summary>
        /// <remarks>This is the RFC2047 decoded value of the name attribute of the Content-Disposition header.</remarks>
        public string Name { get; private set; }

        /// <summary>
        /// A stream containig the value of the boundary.
        /// </summary>
        /// <remarks>This is the RFC2047 decoded value of the Content-Type header.</remarks>
        public HttpMultipartSubStream Value { get; private set; }

        private void ExtractHeaders()
        {
            while(true)
            {
                var header =
                    this.ReadLineFromStream();

                if (string.IsNullOrEmpty(header))
                {
                    break;
                }

                if (header.StartsWith("Content-Disposition", StringComparison.CurrentCultureIgnoreCase))
                {
                    this.Name = Regex.Match(header, @"name=""?(?<name>[^\""]*)", RegexOptions.IgnoreCase).Groups["name"].Value;
                    this.Filename = Regex.Match(header, @"filename=""?(?<filename>[^\""]*)", RegexOptions.IgnoreCase).Groups["filename"].Value;
                }

                if (header.StartsWith("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.ContentType = header.Split(new[] { ' ' }).Last().Trim();
                }
            }

            this.Value.PositionStartAtCurrentLocation();
        }

        private string ReadLineFromStream()
        {
            var readBuffer = new StringBuilder();

            while (true)
            {
                var byteReadFromStream = this.Value.ReadByte();

                if (byteReadFromStream == -1)
                {
                    return null;
                }

                if (byteReadFromStream.Equals(LF))
                {
                    break;
                }

                readBuffer.Append((char)byteReadFromStream);
            }

            var lineReadFromStream =
                readBuffer.ToString().Trim(new[] { (char)CR });

            return lineReadFromStream;
        }
    }
}