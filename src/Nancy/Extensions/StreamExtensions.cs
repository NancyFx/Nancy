namespace Nancy.Extensions
{
    using System;
    using System.IO;

    /// <summary>
    /// Containing extensions for the <see cref="Stream"/> object.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Buffer size for copy operations
        /// </summary>
        internal const int BufferSize = 4096;

        /// <summary>
        /// Copies the contents between two <see cref="Stream"/> instances in an async fashion.
        /// </summary>
        /// <param name="source">The source stream to copy from.</param>
        /// <param name="destination">The destination stream to copy to.</param>
        /// <param name="onComplete">Delegate that should be invoked when the operation has completed. Will pass the source, destination and exception (if one was thrown) to the function. Can pass in <see langword="null" />.</param>
        public static void CopyTo(this Stream source, Stream destination, Action<Stream, Stream, Exception> onComplete)
        {
            var buffer = 
                new byte[BufferSize];

            Action<Exception> done = e =>
            {
                if (onComplete != null)
                {
                    onComplete.Invoke(source, destination, e);
                }
            };

            AsyncCallback rc = null;
            
            rc = readResult =>
            {
                try
                {
                    var read = 
                        source.EndRead(readResult);

                    if (read <= 0)
                    {
                        done.Invoke(null);
                        return;
                    }

                    destination.BeginWrite(buffer, 0, read, writeResult =>
                    {
                        try
                        {
                            destination.EndWrite(writeResult);
                            source.BeginRead(buffer, 0, buffer.Length, rc, null);
                        }
                        catch (Exception ex)
                        {
                            done.Invoke(ex);
                        }

                    }, null);
                }
                catch (Exception ex)
                {
                    done.Invoke(ex);
                }
            };

            source.BeginRead(buffer, 0, buffer.Length, rc, null);
        }
    }
}