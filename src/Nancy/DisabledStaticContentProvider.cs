namespace Nancy
{
    /// <summary>
    /// A "disabled" static content provider - always returns null
    /// so no content is served.
    /// </summary>
    public class DisabledStaticContentProvider : IStaticContentProvider
    {
        /// <summary>
        /// Gets the static content response, if possible.
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Response if serving content, null otherwise</returns>
        public Response GetContent(NancyContext context)
        {
            return null;
        }
    }
}