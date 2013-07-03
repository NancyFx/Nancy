namespace Nancy.Diagnostics
{
    using System;
    using System.Text;

    public class TraceLog
    {
        private readonly StringBuilder log;

        public TraceLog(bool enabled)
        {
            if (enabled)
            {
                this.log = new StringBuilder();
            }
        }

        public TraceLog()
            : this(StaticConfiguration.EnableRequestTracing)
        {
        }

        public void WriteLog(Action<StringBuilder> logDelegate)
        {
            if (this.log != null)
            {
                logDelegate.Invoke(this.log);
            }
        }

        public override string ToString()
        {
            return this.log != null ? this.log.ToString() : string.Empty;
        }
    }
}