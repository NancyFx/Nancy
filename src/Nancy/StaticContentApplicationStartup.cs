namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Conventions;
    using Nancy.Bootstrapper;

    /// <summary>
    /// Registers the static contents hook in the application pipeline at startup.
    /// </summary>
    public class StaticContentApplicationStartup : IApplicationStartup
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly StaticContentsConventions conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticContentApplicationStartup"/> class, using the
        /// provided <paramref name="rootPathProvider"/> and <paramref name="conventions"/>.
        /// </summary>
        /// <param name="rootPathProvider">The current root path provider.</param>
        /// <param name="conventions">The static content conventions.</param>
        public StaticContentApplicationStartup(IRootPathProvider rootPathProvider, StaticContentsConventions conventions)
        {
            this.rootPathProvider = rootPathProvider;
            this.conventions = conventions;
        }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        public void Initialize(IPipelines pipelines)
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