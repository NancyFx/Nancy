namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Provides a hook to execute code during request startup.
    /// </summary>
    public interface IRequestStartup
    {
        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        /// <param name="context">The current context</param>
        void Initialize(IPipelines pipelines, NancyContext context);
    }
}