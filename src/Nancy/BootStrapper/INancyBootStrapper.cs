namespace Nancy.BootStrapper
{
    /// <summary>
    /// BootStrapper for the Nancy Engine
    /// </summary>
    public interface INancyBootStrapper
    {
        /// <summary>
        /// Gets the configured INancyEngine
        /// </summary>
        /// <returns>Configured INancyEngine</returns>
        INancyEngine GetEngine();
    }
}