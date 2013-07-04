namespace Nancy.Diagnostics
{
    using System;
    using System.Text;

    /// <summary>
    /// Provides request trace logging.
    /// Uses a delegate to write to the log, rather than creating strings regardless
    /// of whether the log is enabled or not.
    /// </summary>
    public interface ITraceLog
    {
        /// <summary>
        /// Write to the log
        /// </summary>
        /// <param name="logDelegate">Log writing delegate</param>
        void WriteLog(Action<StringBuilder> logDelegate);
    }
}