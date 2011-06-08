namespace Nancy.ViewEngines
{
    /// <summary>
    /// Defines the functionality required to manufacture <see cref="IRenderContext"/> instances.
    /// </summary>
    public interface IRenderContextFactory
    {
        /// <summary>
        /// Gets a <see cref="IRenderContext"/> for the specified <see cref="ViewLocationContext"/>.
        /// </summary>
        /// <param name="viewLocationContext">The <see cref="ViewLocationContext"/> for which the context should be created.</param>
        /// <returns>A <see cref="IRenderContext"/> instance.</returns>
        IRenderContext GetRenderContext(ViewLocationContext viewLocationContext);
    }
}