namespace Nancy.Diagnostics
{
    using System;
    using System.Text;

    /// <summary>
    /// Default implementation of the <see cref="ITraceLog"/> interface.
    /// </summary>
    public class DefaultTraceLog : ITraceLog
    {
        private readonly StringBuilder log;

        /// <summary>
        /// Creates a new instance of the <see cref="DefaultTraceLog"/> class.
        /// </summary>
        public DefaultTraceLog()
        {
            this.log = new StringBuilder();
        }

        /// <summary>
        /// Write to the log
        /// </summary>
        /// <param name="logDelegate">Log writing delegate</param>
        public void WriteLog(Action<StringBuilder> logDelegate)
        {
            if (this.log != null)
            {
                logDelegate.Invoke(this.log);
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.log != null ? this.log.ToString() : string.Empty;
        }
    }
}