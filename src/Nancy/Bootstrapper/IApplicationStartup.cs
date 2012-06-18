namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Provides a hook to execute code during application startup.
    /// </summary>
    public interface IApplicationStartup
    {
        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        void Initialize(IPipelines pipelines);
    }
}