namespace Nancy.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Nancy.Extensions;

    /// <summary>
    /// A <see cref="Stream"/> decorator that can handle moving the stream out from memory and on to disk when the contents reaches a certain length.
    /// </summary>
    public class RequestStream : Stream
    {
        public static long DEFAULT_SWITCHOVER_THRESHOLD = 81920;

        private bool disableStreamSwitching;
        private readonly long thresholdLength;
        private bool isSafeToDisposeStream;
        private Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestStream"/> class.
        /// </summary>
        /// <param name="expectedLength">The expected length of the contents in the stream.</param>
        /// <param name="thresholdLength">The content length that will trigger the stream to be moved out of memory.</param>
        /// <param name="disableStreamSwitching">if set to <see langword="true"/> the stream will never explicitly be moved to disk.</param>
        public RequestStream(long expectedLength, long thresholdLength, bool disableStreamSwitching)
            : this(null, expectedLength, thresholdLength, disableStreamSwitching)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestStream"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that should be handled by the request stream</param>
        /// <param name="expectedLength">The expected length of the contents in the stream.</param>
        /// <param name="disableStreamSwitching">if set to <see langword="true"/> the stream will never explicitly be moved to disk.</param>
        public RequestStream(Stream stream, long expectedLength, bool disableStreamSwitching)
            : this(stream, expectedLength, DEFAULT_SWITCHOVER_THRESHOLD, disableStreamSwitching)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestStream"/> class.
        /// </summary>
        /// <param name="expectedLength">The expected length of the contents in the stream.</param>
        /// <param name="disableStreamSwitching">if set to <see langword="true"/> the stream will never explicitly be moved to disk.</param>
        public RequestStream(long expectedLength, bool disableStreamSwitching)
            : this(null, expectedLength, DEFAULT_SWITCHOVER_THRESHOLD, disableStreamSwitching)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestStream"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that should be handled by the request stream</param>
        /// <param name="expectedLength">The expected length of the contents in the stream.</param>
        /// <param name="thresholdLength">The content length that will trigger the stream to be moved out of memory.</param>
        /// <param name="disableStreamSwitching">if set to <see langword="true"/> the stream will never explicitly be moved to disk.</param>
        public RequestStream(Stream stream, long expectedLength, long thresholdLength, bool disableStreamSwitching)
        {
            this.thresholdLength = thresholdLength;
            this.disableStreamSwitching = disableStreamSwitching;
            this.stream = stream ?? this.CreateDefaultMemoryStream(expectedLength);

            ThrowExceptionIfCtorParametersWereInvalid(this.stream, expectedLength, this.thresholdLength);

            if (!this.MoveStreamOutOfMemoryIfExpectedLengthExceedSwitchLength(expectedLength))
            {
                this.MoveStreamOutOfMemoryIfContentsLengthExceedThresholdAndSwitchingIsEnabled();
            }

            if (!this.stream.CanSeek)
            {
                var task =
                    MoveToWritableStream();

                task.Wait();

                if (task.IsFaulted)
                {
                    throw new InvalidOperationException("Unable to copy stream", task.Exception);
                }
            }

            this.stream.Position = 0;
        }

        private Task<object> MoveToWritableStream()
        {
            var tcs = new TaskCompletionSource<object>();

            var sourceStream = this.stream;
            this.stream = new MemoryStream(StreamExtensions.BufferSize);

            sourceStream.CopyTo(this, (source, destination, ex) =>
            {
                if (ex != null)
                {
                    tcs.SetException(ex);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public override bool CanSeek
        {
            get { return this.stream.CanSeek; }
        }

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        /// <returns>Always returns <see langword="false"/>.</returns>
        public override bool CanTimeout
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        public override long Length
        {
            get { return this.stream.Length; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream is stored in memory.
        /// </summary>
        /// <value><see langword="true"/> if the stream is stored in memory; otherwise, <see langword="false"/>.</value>
        /// <remarks>The stream is moved to disk when either the length of the contents or expected content length exceeds the threshold specified in the constructor.</remarks>
        public bool IsInMemory
        {
            get { return !(this.stream.GetType() == typeof(FileStream)); }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <returns>The current position within the stream.</returns>
        public override long Position
        {
            get { return this.stream.Position; }
            set
            {
                if (value < 0)
                    throw new InvalidOperationException("The position of the stream cannot be set to less than zero.");

                if (value > this.Length)
                    throw new InvalidOperationException("The position of the stream cannot exceed the length of the stream.");

                this.stream.Position = value;
            }
        }

        /// <summary>
        /// Begins an asynchronous read operation.
        /// </summary>
        /// <returns>An <see cref="T:System.IAsyncResult"/> that represents the asynchronous read, which could still be pending.</returns>
        /// <param name="buffer">The buffer to read the data into. </param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing data read from the stream. </param>
        /// <param name="count">The maximum number of bytes to read. </param>
        /// <param name="callback">An optional asynchronous callback, to be called when the read is complete. </param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous read request from other requests. </param>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.stream.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Begins an asynchronous write operation.
        /// </summary>
        /// <returns>An <see cref="IAsyncResult"/> that represents the asynchronous write, which could still be pending.</returns>
        /// <param name="buffer">The buffer to write data from. </param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> from which to begin writing. </param>
        /// <param name="count">The maximum number of bytes to write. </param>
        /// <param name="callback">An optional asynchronous callback, to be called when the write is complete. </param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous write request from other requests.</param>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.stream.BeginWrite(buffer, offset, count, callback, state);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.isSafeToDisposeStream)
            {
                ((IDisposable)this.stream).Dispose();

                var fileStream = this.stream as FileStream;
                if (fileStream != null)
                {
                    DeleteTemporaryFile(fileStream.Name);
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Waits for the pending asynchronous read to complete.
        /// </summary>
        /// <returns>
        /// The number of bytes read from the stream, between zero (0) and the number of bytes you requested. Streams return zero (0) only at the end of the stream, otherwise, they should block until at least one byte is available.
        /// </returns>
        /// <param name="asyncResult">The reference to the pending asynchronous request to finish. </param>
        public override int EndRead(IAsyncResult asyncResult)
        {
            return this.stream.EndRead(asyncResult);
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request. </param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.stream.EndWrite(asyncResult);

            this.ShiftStreamToFileStreamIfNecessary();
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            this.stream.Flush();
        }

        public static RequestStream FromStream(Stream stream)
        {
            return FromStream(stream, 0, DEFAULT_SWITCHOVER_THRESHOLD, false);
        }

        public static RequestStream FromStream(Stream stream, long expectedLength)
        {
            return FromStream(stream, expectedLength, DEFAULT_SWITCHOVER_THRESHOLD, false);
        }

        public static RequestStream FromStream(Stream stream, long expectedLength, long thresholdLength)
        {
            return FromStream(stream, expectedLength, thresholdLength, false);
        }

        public static RequestStream FromStream(Stream stream, long expectedLength, bool disableStreamSwitching)
        {
            return FromStream(stream, expectedLength, DEFAULT_SWITCHOVER_THRESHOLD, disableStreamSwitching);
        }

        public static RequestStream FromStream(Stream stream, long expectedLength, long thresholdLength, bool disableStreamSwitching)
        {
            return new RequestStream(stream, expectedLength, thresholdLength, disableStreamSwitching);
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source. </param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream. </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream. </param>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        /// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
        public override int ReadByte()
        {
            return this.stream.ReadByte();
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <returns>The new position within the current stream.</returns>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter. </param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position. </param>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.stream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes. </param>
        /// <exception cref="NotSupportedException">The stream does not support having it's length set.</exception>
        /// <remarks>This functionality is not supported by the <see cref="RequestStream"/> type and will always throw <see cref="NotSupportedException"/>.</remarks>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream. </param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream. </param>
        /// <param name="count">The number of bytes to be written to the current stream. </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);

            this.ShiftStreamToFileStreamIfNecessary();
        }

        private void ShiftStreamToFileStreamIfNecessary()
        {
            if (this.disableStreamSwitching)
            {
                return;
            }

            if (this.stream.Length >= this.thresholdLength)
            {
                // Close the stream here as closing it every time we call
                // MoveStreamContentsToFileStream causes an (ObjectDisposedException)
                // in NancyWcfGenericService - webRequest.UriTemplateMatch
                var old = this.stream;
                this.MoveStreamContentsToFileStream();
                old.Close();
            }
        }

        private static FileStream CreateTemporaryFileStream()
        {
            var filePath = Path.GetTempFileName();

            return new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 8192, StaticConfiguration.AllowFileStreamUploadAsync);
        }

        private Stream CreateDefaultMemoryStream(long expectedLength)
        {
            this.isSafeToDisposeStream = true;

            if (this.disableStreamSwitching || expectedLength < this.thresholdLength)
            {
                return new MemoryStream((int)expectedLength);
            }

            this.disableStreamSwitching = true;

            return CreateTemporaryFileStream();
        }

        private static void DeleteTemporaryFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }

            try
            {
                File.Delete(fileName);
            }
            catch
            {
            }
        }

        private void MoveStreamOutOfMemoryIfContentsLengthExceedThresholdAndSwitchingIsEnabled()
        {
            if (!this.stream.CanSeek)
            {
                return;
            }

            try
            {
                if ((this.stream.Length > this.thresholdLength) && !this.disableStreamSwitching)
                {
                    this.MoveStreamContentsToFileStream();
                }
            }
            catch (NotSupportedException)
            {
            }
        }

        private bool MoveStreamOutOfMemoryIfExpectedLengthExceedSwitchLength(long expectedLength)
        {
            if ((expectedLength >= this.thresholdLength) && !this.disableStreamSwitching)
            {
                this.MoveStreamContentsToFileStream();
                return true;
            }
            return false;
        }

        private void MoveStreamContentsToFileStream()
        {
            var targetStream = CreateTemporaryFileStream();
            this.isSafeToDisposeStream = true;

            if (this.stream.CanSeek && this.stream.Length == 0)
            {
                this.stream.Close();
                this.stream = targetStream;
                return;
            }

            // Seek to 0 if we can, although if we can't seek, and we've already written (if the size is unknown) then
            // we are screwed anyway, and some streams that don't support seek also don't let you read the position so
            // there's no real way to check :-/
            if (this.stream.CanSeek)
            {
                this.stream.Position = 0;
            }
            this.stream.CopyTo(targetStream, 8196);
            if (this.stream.CanSeek)
            {
                this.stream.Flush();
            }

            this.stream = targetStream;

            this.disableStreamSwitching = true;
        }

        private static void ThrowExceptionIfCtorParametersWereInvalid(Stream stream, long expectedLength, long thresholdLength)
        {
            if (!stream.CanRead)
            {
                throw new InvalidOperationException("The stream must support reading.");
            }

            if (expectedLength < 0)
            {
                throw new ArgumentOutOfRangeException("expectedLength", expectedLength, "The value of the expectedLength parameter cannot be less than zero.");
            }

            if (thresholdLength < 0)
            {
                throw new ArgumentOutOfRangeException("thresholdLength", thresholdLength, "The value of the threshHoldLength parameter cannot be less than zero.");
            }
        }

    }
}
