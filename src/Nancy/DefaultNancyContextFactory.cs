namespace Nancy
{
    /// <summary>
    /// Creates NancyContext instances
    /// </summary>
    public class DefaultNancyContextFactory : INancyContextFactory
    {
        /// <summary>
        /// Create a new NancyContext
        /// </summary>
        /// <returns>NancyContext instance</returns>
        public NancyContext Create()
        {
            var nancyContext = new NancyContext();

            nancyContext.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("New Request Started"));

            return nancyContext;
        }
    }
}