namespace Nancy.Hosting.Owin.Fakes
{
    using System;
    using System.IO;
    using System.Threading;
    using BodyDelegate = System.Func<System.Func<System.ArraySegment<byte>, // data
                                 System.Action,                         // continuation
                                 bool>,                                 // continuation will be invoked
                                 System.Action<System.Exception>,       // onError
                                 System.Action,                         // on Complete
                                 System.Action>;                        // cancel

    /// <summary>
    /// Consumes a body delegate
    /// </summary>
    public class FakeConsumer
    {
        private readonly bool useContinuation = false;

        private Action cancelDelegate;

        private bool bodyDelegateInvoked;

        private MemoryStream dataStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeConsumer"/> class.
        /// </summary>
        /// <param name="useContinuation">Whether to use async/the continuation if supplied.</param>
        public FakeConsumer(bool useContinuation)
        {
            this.useContinuation = useContinuation;
        }

        /// <summary>
        /// Gets a value indicating whether complete has been called.
        /// </summary>
        public bool CompleteCalled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a non-null continuation was sent by the producer.
        /// </summary>
        public bool ContinuationSent { get; private set; }

        /// <summary>
        /// Gets the data that was consumed - only valid after complete called.
        /// </summary>
        public byte[] ConsumedData { get; private set; }

        /// <summary>
        /// Gets the exeption that was thrown - only set if the error delegate was invoked
        /// </summary>
        public Exception RaisedException { get; private set; }

        /// <summary>
        /// Invoke the body delegate
        /// </summary>
        /// <param name="bodyDelegate">The body delegate to invoke</param>
        public void InvokeBodyDelegate(BodyDelegate bodyDelegate)
        {
            if (bodyDelegate == null)
            {
                throw new ArgumentNullException("bodyDelegate");    
            }

            this.dataStream = new MemoryStream();
            this.cancelDelegate = bodyDelegate.Invoke(this.DataConsumer, this.OnError, this.OnComplete);
            this.bodyDelegateInvoked = true;
        }

        /// <summary>
        /// Invokes the cancel delegate returned from the body delegate
        /// </summary>
        public void InvokeCancel()
        {
            if (!this.bodyDelegateInvoked)
            {
                throw new InvalidOperationException("Body delegate must be invoked before it can be cancelled!");
            }

            this.cancelDelegate.Invoke();
        }

        private void OnComplete()
        {
            this.CompleteCalled = true;
            this.dataStream.Close();
            this.ConsumedData = this.dataStream.ToArray();
        }

        private void OnError(Exception ex)
        {
            this.RaisedException = ex;
            this.dataStream.Dispose();
        }

        private bool DataConsumer(ArraySegment<byte> data, Action continuation)
        {
            this.ContinuationSent = continuation != null;

            if (continuation == null || !this.useContinuation)
            {
                // No continuation - consume sync.
                // and return false to indicate we won't be calling the continuation
                this.ConsumeDataSync(data);

                return false;
            }

            // Continuation is to be used, execute the data read
            // on a background thread and return true to indicate
            // that we will be calling the continuation.
            this.ConsumeDataAsync(data, continuation);

            return true;
        }

        private void ConsumeDataSync(ArraySegment<byte> data)
        {
            this.dataStream.Write(data.Array, data.Offset, data.Count);
        }

        private void ConsumeDataAsync(ArraySegment<byte> data, Action continuation)
        {
            // We don't us the thread pool to try and stop it being clever
            // and running us sync.
            var worker = new Thread(
                ts =>
                    {
                        this.ConsumeDataSync(data);
                        continuation.Invoke();
                    });

            worker.Start();
        }
    }
}