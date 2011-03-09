namespace Nancy
{
    using System;
    using System.IO;

    public class HttpMultiparSubStream : Stream
    {
        private readonly Stream stream;
        private long start;
        private readonly long end;
        private long position;

        public HttpMultiparSubStream(Stream stream, long start, long end)
        {
            this.stream = stream;
            this.start = start;
            this.position = start;
            this.end = end;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return (this.end - this.start); }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception><exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
        public override long Position
        {
            get { return this.position - this.start; }
            set { this.position = this.Seek(value, SeekOrigin.Begin); }
        }

        private long CalculateSubStreamRelativePosition(SeekOrigin origin, long offset)
        {
            var subStreamRelativePosition = 0L;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    subStreamRelativePosition = this.start + offset;
                    break;

                case SeekOrigin.Current:
                    subStreamRelativePosition = this.position + offset;
                    break;

                case SeekOrigin.End:
                    subStreamRelativePosition = this.end + offset;
                    break;
            }
            return subStreamRelativePosition;
        }

        public void PositionStartAtCurrentLocation()
        {
            this.start = this.stream.Position;
        }

        public override void Flush()
        {
        }

        /// <remarks>Will reposition the wrapped stream.</remarks>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > (this.end - this.position))
            {
                count = (int)(this.end - this.position);
            }

            if (count <= 0)
            {
                return 0;
            }

            this.stream.Position = this.position;

            var bytesReadFromStream =
                this.stream.Read(buffer, offset, count);

            this.RepositionAfterRead(bytesReadFromStream);

            return bytesReadFromStream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>Will reposition the wrapped stream.</remarks>
        public override int ReadByte()
        {
            if (this.position >= this.end)
            {
                return -1;
            }

            this.stream.Position = this.position;

            var byteReadFromStream = this.stream.ReadByte();
            
            this.RepositionAfterRead(1);

            return byteReadFromStream;
        }

        private void RepositionAfterRead(int bytesReadFromStream)
        {
            if (bytesReadFromStream == -1)
            {
                this.position = this.end;
            }
            else
            {
                this.position += bytesReadFromStream;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var subStreamRelativePosition = 
                this.CalculateSubStreamRelativePosition(origin, offset);

            this.ThrowExceptionIsPositionIsOutOfBounds(subStreamRelativePosition);

            this.position = this.stream.Seek(subStreamRelativePosition, SeekOrigin.Begin);

            return this.position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        private void ThrowExceptionIsPositionIsOutOfBounds(long subStreamRelativePosition)
        {
            if (subStreamRelativePosition > this.Length)
            {
                var ingdfgd = 10;
            }

            if (subStreamRelativePosition < 0 || subStreamRelativePosition > this.end)
                throw new InvalidOperationException();
        }
    }
}