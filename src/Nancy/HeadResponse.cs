namespace Nancy
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

 	/// <summary>
    /// Represents a HEAD only response.
    /// </summary>
    public class HeadResponse : Response
    {
        private const string ContentLength = "Content-Length";
        private readonly Response innerResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadResponse"/> class.
        /// </summary>
        /// <param name="response">
        /// The full response to create the head response from.
        /// </param>
        public HeadResponse(Response response)
        {
            this.innerResponse = response;
            this.Contents = stream =>
            {
                this.CheckAndSetContentLength(this.innerResponse);
                GetStringContents(string.Empty)(stream);
            };
            this.ContentType = response.ContentType;
            this.Headers = response.Headers;
            this.StatusCode = response.StatusCode;
            this.ReasonPhrase = response.ReasonPhrase;
        }


        /// <summary>
        /// Executes at the end of the nancy execution pipeline and before control is passed back to the hosting.
        /// Can be used to pre-render/validate views while still inside the main pipeline/error handling.
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <returns>
        /// Task for completion/erroring
        /// </returns>
        public override Task PreExecute(NancyContext context)
        {
            return this.innerResponse.PreExecute(context);
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

#if !NETSTANDARD1_6
            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                throw new NotSupportedException();
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                throw new NotSupportedException();
            }
#endif

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