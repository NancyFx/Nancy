namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    public class RequestTraceSession
    {
        private const int MaxSize = 500;
        private readonly ConcurrentLimitedCollection<IRequestTrace> requestTraces;

        public RequestTraceSession(Guid id)
        {
            this.Id = id;
            this.requestTraces = new ConcurrentLimitedCollection<IRequestTrace>(MaxSize);
        }

        public Guid Id { get; private set; }

        public IEnumerable<IRequestTrace> RequestTraces
        {
            get
            {
                return this.requestTraces;
            }
        }

        public void AddRequestTrace(IRequestTrace trace)
        {
            this.requestTraces.Add(trace);
        }
    }
}