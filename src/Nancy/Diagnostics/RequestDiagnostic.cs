namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    public class RequestDiagnostic
    {
        public string Method { get; set; }

        public Url RequestUrl { get; set; }

        public TraceLog TraceLog { get; private set; }

        public IDictionary<string, object> Items { get; private set; }

        public RequestDiagnostic()
        {
            this.TraceLog = new TraceLog();
            this.Items = new Dictionary<string, object>();
        }
    }
}