namespace Nancy
{
    using System;
    using System.Linq;

    /// <summary>
    /// A buffer that is used to locate a HTTP multipart/form-data boundary in a stream.
    /// </summary>
    public class HttpMultipartBuffer
    {
        private readonly byte[] boundaryAsBytes;
        private readonly byte[] closingBoundaryAsBytes;
        private readonly byte[] buffer;
        private int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMultipartBuffer"/> class.
        /// </summary>
        /// <param name="boundaryAsBytes">The boundary as a byte-array.</param>
        /// <param name="closingBoundaryAsBytes">The closing boundary as byte-array</param>
        public HttpMultipartBuffer(byte[] boundaryAsBytes, byte[] closingBoundaryAsBytes)
        {
            this.boundaryAsBytes = boundaryAsBytes;
            this.closingBoundaryAsBytes = closingBoundaryAsBytes;
            this.buffer = new byte[this.boundaryAsBytes.Length];
        }

        /// <summary>
        /// Gets a value indicating whether the buffer contains the same values as the boundary.
        /// </summary>
        /// <value><see langword="true"/> if buffer contains the same values as the boundary; otherwise, <see langword="false"/>.</value>
        public bool IsBoundary
        {
            get { return this.buffer.SequenceEqual(this.boundaryAsBytes); }
        }
        public bool IsClosingBoundary
        {
            get { return this.buffer.SequenceEqual(this.closingBoundaryAsBytes); }
        }
        /// <summary>
        /// Gets a value indicating whether this buffer is full.
        /// </summary>
        /// <value><see langword="true"/> if buffer is full; otherwise, <see langword="false"/>.</value>
        public bool IsFull
        {
            get { return this.position.Equals(this.buffer.Length); }
        }

        /// <summary>
        /// Gets the number of bytes that can be stored in the buffer.
        /// </summary>
        /// <value>The number of bytes that can be stored in the buffer.</value>
        public int Length
        {
            get { return this.buffer.Length; }
        }

        /// <summary>
        /// Resets the buffer so that inserts happens from the start again.
        /// </summary>
        /// <remarks>This does not clear any previously written data, just resets the buffer position to the start. Data that is inserted after Reset has been called will overwrite old data.</remarks>
        public void Reset()
        {
            this.position = 0;
        }

        /// <summary>
        /// Inserts the specified value into the buffer and advances the internal position.
        /// </summary>
        /// <param name="value">The value to insert into the buffer.</param>
        /// <remarks>This will throw an <see cref="ArgumentOutOfRangeException"/> is you attempt to call insert more times then the <see cref="Length"/> of the buffer and <see cref="Reset"/> was not invoked.</remarks>
        public void Insert(byte value)
        {
            this.buffer[this.position++] = value;
        }
    }
}