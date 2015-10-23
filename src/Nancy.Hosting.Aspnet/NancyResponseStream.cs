namespace Nancy.Hosting.Aspnet
{
    using System.IO;
    using System.Web;

    /// <summary>
    /// A wrapper around an AspNet OutputStream that defers .Flush() to the parent HttpResponseBase
    /// </summary>
    public class NancyResponseStream : Stream
    {
        private readonly HttpResponseBase response;

        public NancyResponseStream(HttpResponseBase response)
        {
            this.response = response;
        }
        public override bool CanRead
        {
            get { return this.response.OutputStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.response.OutputStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return this.response.OutputStream.CanWrite; }
        }

        /// <summary>
        /// Calls Flush() on the wrapped HttpResponseBase
        /// </summary>
        public override void Flush()
        {
            this.response.Flush();
        }

        public override long Length
        {
            get { return this.response.OutputStream.Length; }
        }

        public override long Position
        {
            get
            {
                return this.response.OutputStream.Position;
            }
            set
            {
                this.response.OutputStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.response.OutputStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.response.OutputStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.response.OutputStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.response.OutputStream.Write(buffer, offset, count);
        }
    }
}
