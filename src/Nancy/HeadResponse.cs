namespace Nancy
{
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Represents a HEAD only response.
    /// </summary>
	public class HeadResponse : Response
	{
        private const string ContentLength = "Content-Length";

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadResponse"/> class.
        /// </summary>
        /// <param name="response">
        /// The full response to create the head response from.
        /// </param>
        public HeadResponse(Response response)
        {
            this.Contents = GetStringContents(string.Empty);
            this.ContentType = response.ContentType;
            this.Headers = response.Headers;
            this.StatusCode = response.StatusCode;
            this.CheckAndSetContentLength(response);
        }

        private void CheckAndSetContentLength(Response response)
        {
            if (this.Headers.ContainsKey(ContentLength))
            {
                return;
            }

            using (var nullStream = new NullStream())
            {
                response.Contents.Invoke(nullStream);

                this.Headers[ContentLength] = nullStream.Length.ToString(CultureInfo.InvariantCulture);
            }

        }

        private sealed class NullStream : Stream
        {
            private int bytesWritten;

            public override void Flush()
            {
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                throw new NotSupportedException();
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override int ReadByte()
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                // We assume we can't seek and can't overwrite, but don't throw just in case.
                this.bytesWritten += count;
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanTimeout
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override long Length
            {
                get { return this.bytesWritten; }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }
        }
    }
}