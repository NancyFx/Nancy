namespace Nancy
{
    using Nancy.Bootstrapper;
    using Nancy.Responses;

    /// <summary>
    /// Assigns the root path of the application whom ever needs it.
    /// </summary>
    /// <remarks>This task is run at application startup.</remarks>
    public class RootPathApplicationStartup : IApplicationStartup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootPathApplicationStartup"/> class.
        /// </summary>
        /// <param name="rootPathProvider">An <see cref="IRootPathProvider"/> instance.</param>
        public RootPathApplicationStartup(IRootPathProvider rootPathProvider)
        {
            GenericFileResponse.SafePaths.Add(rootPathProvider.GetRootPath());
        }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        public void Initialize(IPipelines pipelines)
        {
        }
    }
}