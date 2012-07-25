namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    public class RequestTraceSession
    {
        private const int MaxSize = 500;

        private readonly ConcurrentLimitedCollection<RequestTrace> requestTraces;

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
            this.requestTraces = new ConcurrentLimitedCollection<RequestTrace>(MaxSize);
        }

        public void AddRequestTrace(RequestTrace trace)
        {
            this.requestTraces.Add(trace);
        }
    }
}