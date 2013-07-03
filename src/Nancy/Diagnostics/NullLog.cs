namespace Nancy.Diagnostics
{
    using System;
    using System.Text;

    public class NullLog : ITraceLog
    {
        public void WriteLog(Action<StringBuilder> logDelegate)
        {
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}