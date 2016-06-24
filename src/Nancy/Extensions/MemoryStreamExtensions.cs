namespace Nancy.Extensions
{
    using System;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public static class MemoryStreamExtensions
    {
        /// <summary>
        /// Gets the buffer segment.
        /// </summary>
        /// <param name="stream">The memory stream.</param>
        /// <returns>Buffer segment as bytes</returns>
        public static ArraySegment<byte> GetBufferSegment(this MemoryStream stream)
        {
#if NETSTANDARD1_6
            ArraySegment<byte> buffer;
            if (stream.TryGetBuffer(out buffer))
            {
                return buffer;
            }
#endif
            var bytes = stream.GetBytes();

            return new ArraySegment<byte>(bytes, 0, bytes.Length);
        }

        private static byte[] GetBytes(this MemoryStream stream)
        {
#if NETSTANDARD1_6
            return stream.ToArray(); // This is all we have if TryGetBuffer fails in GetBufferSegment
#else
            return stream.GetBuffer();
#endif
        }
    }
}
