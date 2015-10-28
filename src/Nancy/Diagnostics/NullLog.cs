namespace Nancy.Diagnostics
{
    using System;
    using System.Text;

    /// <summary>
    /// Implementation of <see cref="ITraceLog"/> that does not log anything.
    /// </summary>
    public class NullLog : ITraceLog
    {
        /// <summary>
        /// Write to the log
        /// </summary>
        /// <param name="logDelegate">Log writing delegate</param>
        public void WriteLog(Action<StringBuilder> logDelegate)
        {
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Empty;
        }
    }
}