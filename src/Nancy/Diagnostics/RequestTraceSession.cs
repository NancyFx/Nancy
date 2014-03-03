namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    public class RequestTraceSession
    {
        private const int MaxSize = 500;

        private readonly ConcurrentLimitedCollection<DefaultRequestTrace> requestTraces;

        public Guid Id { get; private set; }

        public IEnumerable<DefaultRequestTrace> RequestTraces
        {
            get
            {
                return this.requestTraces;
            }
        }

        public RequestTraceSession(Guid id)
        {
            this.Id = id;
            this.requestTraces = new ConcurrentLimitedCollection<DefaultRequestTrace>(MaxSize);
        }

        public void AddRequestTrace(DefaultRequestTrace trace)
        {
            this.requestTraces.Add(trace);
        }
    }
}