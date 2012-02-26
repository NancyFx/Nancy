namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    public class RequestTraceSession
    {
        private readonly object diagsLock = new object();

        private readonly List<RequestTrace> requestTraces;

        public Guid Id { get; private set; }

        public IEnumerable<RequestTrace> RequestTraces
        {
            get
            {
                return this.requestTraces;
            }
        }

        public RequestTraceSession(Guid id)
        {
            this.Id = id;
            this.requestTraces = new List<RequestTrace>();
        }

        public void AddRequestTrace(RequestTrace trace)
        {
            lock (this.diagsLock)
            {
                this.requestTraces.Add(trace);
            }
        }
    }
}