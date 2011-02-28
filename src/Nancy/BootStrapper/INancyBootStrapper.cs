namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Bootstrapper for the Nancy Engine
    /// </summary>
    public interface INancyBootstrapper
    {
        /// <summary>
        /// Gets the configured INancyEngine
        /// </summary>
        /// <returns>Configured INancyEngine</returns>
        INancyEngine GetEngine();
    }
}