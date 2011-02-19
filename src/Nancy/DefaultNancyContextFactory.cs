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
            return new NancyContext();
        }
    }
}