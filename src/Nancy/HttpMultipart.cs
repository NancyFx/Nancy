namespace Nancy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Retrieves <see cref="HttpMultipartBoundary"/> instances from a request stream.
    /// </summary>
    public class HttpMultipart
    {
        private const byte LF = (byte)'\n';
        private readonly byte[] boundaryAsBytes;
        private readonly HttpMultipartBuffer readBuffer;
        private readonly Stream requestStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMultipart"/> class.
        /// </summary>
        /// <param name="requestStream">The request stream to parse.</param>
        /// <param name="boundary">The boundary marker to look for.</param>
        public HttpMultipart(Stream requestStream, string boundary)
        {
            this.requestStream = requestStream;
            this.boundaryAsBytes = GetBoundaryAsBytes(boundary);
            this.readBuffer = new HttpMultipartBuffer(this.boundaryAsBytes);
        }

        /// <summary>
        /// Gets the <see cref="HttpMultipartBoundary"/> instances from the request stream.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing the found <see cref="HttpMultipartBoundary"/> instances.</returns>
        public IEnumerable<HttpMultipartBoundary> GetBoundaries()
        {
            return
                (from boundaryStream in this.GetBoundarySubStreams()
                select new HttpMultipartBoundary(boundaryStream)).ToList();
        }

        private IEnumerable<HttpMultipartSubStream> GetBoundarySubStreams()
        {
            var boundarySubStreams = new List<HttpMultipartSubStream>();
            var boundaryStart = this.GetNextBoundaryPosition();

            while (boundaryStart > -1)
            {
                var boundaryEnd = this.GetNextBoundaryPosition();

                boundarySubStreams.Add(new HttpMultipartSubStream(
                    this.requestStream,
                    boundaryStart,
                    this.GetActualEndOfBoundary(boundaryEnd)));

                boundaryStart = boundaryEnd;
            }

            return boundarySubStreams;
        }

        private long GetActualEndOfBoundary(long boundaryEnd)
        {
            if (this.CheckIfFoundEndOfStream())
            {
                return this.requestStream.Position - (this.readBuffer.Length + 2);
            }

            return boundaryEnd - (this.readBuffer.Length + 2);
        }

        private bool CheckIfFoundEndOfStream()
        {
            return this.requestStream.Position.Equals(this.requestStream.Length);
        }

        private static byte[] GetBoundaryAsBytes(string boundary)
        {
            var boundaryBuilder = new StringBuilder();

            boundaryBuilder.Append("--");
            boundaryBuilder.Append(boundary);
            boundaryBuilder.Append('\r');
            boundaryBuilder.Append('\n');

            var bytes =
                Encoding.ASCII.GetBytes(boundaryBuilder.ToString());

            return bytes;
        }

        private long GetNextBoundaryPosition()
        {
            this.readBuffer.Reset();
            while(true)
            {
                var byteReadFromStream = this.requestStream.ReadByte();

                if (byteReadFromStream == -1)
                {
                    return -1;
                }

                this.readBuffer.Insert((byte)byteReadFromStream);

                if (this.readBuffer.IsFull && this.readBuffer.IsBoundary)
                {
                    return this.requestStream.Position;
                }

                if (byteReadFromStream.Equals(LF) || this.readBuffer.IsFull)
                {
                    this.readBuffer.Reset();
                }
            }
        }
    }
}