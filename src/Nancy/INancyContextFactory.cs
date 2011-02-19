namespace Nancy
{
    /// <summary>
    /// Creates NancyContext instances
    /// </summary>
    public interface INancyContextFactory
    {
        /// <summary>
        /// Create a new NancyContext
        /// </summary>
        /// <returns>NancyContext instance</returns>
        NancyContext Create();
    }
}