namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Provides a hook to execute code during request startup.
    /// </summary>
    public interface IRequestStartup
    {
        /// <summary>
        /// Perform any request initialisation tasks
        /// </summary>
        /// <param name="pipelines">Request pipelines</param>
        /// <param name="context">The current context</param>
        void Initialize(IPipelines pipelines, NancyContext context);
    }
}