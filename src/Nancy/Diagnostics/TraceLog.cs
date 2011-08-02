namespace Nancy.Diagnostics
{
    using System;
    using System.Text;

    public class TraceLog
    {
        private readonly StringBuilder log;

        public TraceLog()
        {
            this.log = new StringBuilder();
        }

        public void WriteLog(Action<StringBuilder> logDelegate)
        {
            if (!StaticConfiguration.EnableDiagnostics)
            {
                return;
            }

            logDelegate.Invoke(this.log);
        }

        public override string ToString()
        {
            return this.log.ToString();
        }
    }
}