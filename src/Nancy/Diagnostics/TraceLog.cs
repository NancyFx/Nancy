namespace Nancy.Diagnostics
{
    using System;
    using System.Text;

    public class TraceLog
    {
        private readonly StringBuilder log;

        public TraceLog()
        {
            if (StaticConfiguration.EnableRequestTracing)
            {
                this.log = new StringBuilder();
            }
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