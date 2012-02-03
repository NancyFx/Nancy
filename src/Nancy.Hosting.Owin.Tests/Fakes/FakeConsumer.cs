using Owin;

namespace Nancy.Hosting.Owin.Tests.Fakes
{
    using System;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Consumes a body delegate
    /// </summary>
    public class FakeConsumer
    {
        private readonly bool useContinuation;

        private Action cancelDelegate;

        private bool bodyDelegateInvoked;

        private MemoryStream dataStream;
        private ManualResetEventSlim sync = new ManualResetEventSlim();

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
        public void InvokeBodyDelegate(BodyDelegate bodyDelegate, bool waitForComplete = true)
        {
            if (bodyDelegate == null)
            {
                throw new ArgumentNullException("bodyDelegate");    
            }

            this.sync.Reset();

            this.dataStream = new MemoryStream();
            
            var cts = new CancellationTokenSource();
            bodyDelegate.Invoke(this.OnWrite, this.OnFlush, this.OnEnd, cts.Token);
            this.cancelDelegate = cts.Cancel;

            this.bodyDelegateInvoked = true;

            if (waitForComplete)
            {
                this.sync.Wait();
            }
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

            this.sync.Set();
        }

        private void OnEnd(Exception ex)
        {
            if (ex == null)
            {
                OnComplete();
            }
            else
            {
                OnError(ex);
            }
        }
        
        private void OnComplete()
        {
            this.CompleteCalled = true;
            this.dataStream.Close();
            this.ConsumedData = this.dataStream.ToArray();
            this.sync.Set();
        }

        private void OnError(Exception ex)
        {
            this.RaisedException = ex;
            this.dataStream.Dispose();
            this.sync.Set();
        }

        private bool OnWrite(ArraySegment<byte> data)
        {
            this.ConsumeDataSync(data);

            if (this.useContinuation)
            {
                // return true is continuation will be used, to indicate calling
                // flush for backpressure would be appropriate
                return true;
            }

            // return false to indicate data is not buffering
            return false;
        }

        private bool OnFlush(Action continuation)
        {
            if (continuation == null || !this.useContinuation)
            {
                // No continuation return false to indicate we won't be calling the continuation
                return false;
            }

            // Continuation is to be used, execute the callback
            // on a background thread and return true to indicate
            // that we will be calling the continuation.
            ContinuationAsync(continuation);

            return true;
        }
        

        private void ConsumeDataSync(ArraySegment<byte> data)
        {
            this.dataStream.Write(data.Array, data.Offset, data.Count);
        }

        private static void ContinuationAsync(Action continuation)
        {
            // We don't us the thread pool to try and stop it being clever
            // and running us sync.
            var worker = new Thread(
                ts => continuation.Invoke());

            worker.Start();
        }
    }
}