using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.ViewEngines.Razor
{
    /// <summary>
    /// A read-only stream backed by a <c>TextReader</c>.
    /// From https://github.com/dotlattice/LatticeUtils
    /// Licensed as public domain: https://github.com/dotlattice/LatticeUtils/blob/master/LICENSE.md
    /// </summary>
    public class TextReaderStream : Stream
    {
        private readonly TextReader textReader;
        private readonly Encoding encoding;
        private readonly List<byte> internalBuffer = new List<byte>();

        /// <summary>
        /// Constructs a stream that reads from the specified <c>TextReader</c> encoded to bytes using the <c>UTF8</c> encoding.
        /// </summary>
        /// <param name="textReader">the backing <c>TextReader</c></param>
        /// <exception cref="System.ArgumentNullException">the textReader is null</exception>
        public TextReaderStream(TextReader textReader)
            : this(textReader, encoding: null) { }

        /// <summary>
        /// Constructs a stream that reads from the specified <c>TextReader</c> using the specified encoding.
        /// </summary>
        /// <param name="textReader">the backing <c>TextReader</c></param>
        /// <param name="encoding">the encoding to use; if null then the encoding will default to <c>UTF8</c></param>
        /// <exception cref="System.ArgumentNullException">the textReader is null</exception>
        public TextReaderStream(TextReader textReader, Encoding encoding)
        {
            if (textReader == null) throw new ArgumentNullException("textReader");
            if (encoding == null) encoding = Encoding.UTF8;

            this.textReader = textReader;
            this.encoding = encoding;
        }

        /// <summary>
        /// The encoding used to encode the characters from the backing <c>TextReader</c> as bytes.
        /// </summary>
        public Encoding Encoding { get { return encoding; } }

        /// <summary>
        /// Always true.
        /// </summary>
        public override bool CanRead { get { return true; } }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            FillInternalBufferfNecessary(count);
            if (!internalBuffer.Any())
            {
                return 0;
            }

            int readCount = Math.Min(internalBuffer.Count, count);
            internalBuffer.CopyTo(0, buffer, offset, readCount);
            internalBuffer.RemoveRange(0, readCount);
            return readCount;
        }

        /// <inheritdoc />
        public override int ReadByte()
        {
            FillInternalBufferfNecessary(1);
            if (!internalBuffer.Any())
            {
                return -1;
            }

            int result = (int)internalBuffer[0];
            internalBuffer.RemoveAt(0);
            return result;
        }

        private void FillInternalBufferfNecessary(int count)
        {
            if (internalBuffer.Count >= count)
            {
                // Not necessary
                return;
            }

            int requestedByteCount = count - internalBuffer.Count;

            // We're assuming that each character is at least one byte in the encoding.
            // It's fine if a character is more than one byte; that just means we'll get more 
            // data than we need in the buffer for this request (and we'll use it on the next one).
            int requestedCharCount = requestedByteCount;

            var charBuffer = new char[requestedCharCount];
            int charReadCount = textReader.Read(charBuffer, index: 0, count: charBuffer.Length);
            if (charReadCount > 0)
            {
                var charBytes = encoding.GetBytes(charBuffer, index: 0, count: charReadCount);
                internalBuffer.AddRange(charBytes);
            }
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override void Flush() { }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                textReader.Dispose();
                internalBuffer.Clear();
            }
        }

        #region Not Supported

        /// <summary>
        /// Always false.
        /// </summary>
        public override bool CanSeek { get { return false; } }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        public override long Length { get { throw new NotSupportedException(); } }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        public override void SetLength(long value) { throw new NotSupportedException(); }

        /// <summary>
        /// Always false.
        /// </summary>
        public override bool CanWrite { get { return false; } }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }

        #endregion
    }
}
