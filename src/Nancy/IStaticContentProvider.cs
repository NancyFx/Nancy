namespace Nancy
{
    /// <summary>
    /// Provides static content delivery
    /// </summary>
    public interface IStaticContentProvider
    {
        /// <summary>
        /// Gets the static content response, if possible.
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Response if serving content, null otherwise</returns>
        Response GetContent(NancyContext context);
    }
}