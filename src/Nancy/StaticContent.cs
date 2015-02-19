namespace Nancy
{
    using System;
    using System.Linq;

    using Nancy.Bootstrapper;
    using Nancy.Conventions;

    /// <summary>
    /// Registers the static contents hook in the application pipeline at startup.
    /// </summary>
    public class StaticContent : IApplicationStartup
    {
        private static IRootPathProvider rootPathProvider;
        private static StaticContentsConventions conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticContent"/> class, using the
        /// provided <paramref name="rootPathProvider"/> and <paramref name="conventions"/>.
        /// </summary>
        /// <param name="rootPathProvider">The current root path provider.</param>
        /// <param name="conventions">The static content conventions.</param>
        public StaticContent(IRootPathProvider rootPathProvider, StaticContentsConventions conventions)
        {
            StaticContent.rootPathProvider = rootPathProvider;
            StaticContent.conventions = conventions;
        }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        public void Initialize(IPipelines pipelines)
        {
        }

        /// <summary>
        /// Enable "manual" static content.
        /// Only use this if you want to manually configure a pipeline hook to have static
        /// content server, for example, after authentication.
        /// </summary>
        /// <param name="pipelines">The pipelines to hook into</param>
        public static void Enable(IPipelines pipelines)
        {
            var item = new PipelineItem<Func<NancyContext, Response>>("Static content", ctx =>
            {
                return conventions
                    .Select(convention => convention.Invoke(ctx, rootPathProvider.GetRootPath()))
                    .FirstOrDefault(response => response != null);
            });

            pipelines.BeforeRequest.AddItemToStartOfPipeline(item);
        }
    }
}