namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Nancy.Bootstrapper;

    public class DiagnosticsStartup : IStartup
    {
        /// <summary>
        /// Gets the type registrations to register for this startup task`
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the collection registrations to register for this startup task
        /// </summary>
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get { return null; }
        }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
            DiagnosticsHook.Enable(pipelines);
        }
    }

    public static class DiagnosticsHook
    {
        public static void Enable(IPipelines pipelines)
        {
            //if (!StaticConfiguration.EnableDiagnostics)
            //{
            //    return;
            //}

            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx =>
            {
                if (ctx.Request.Path.StartsWith("/_Nancy/Diagnostics/Resources/", StringComparison.OrdinalIgnoreCase))
                {
                    return new EmbeddedFileResponse(
                        typeof(DiagnosticsHook).Assembly,
                        "Nancy.Diagnostics.Resources",
                        Path.GetFileName(ctx.Request.Url.Path)
                    );
                }

                return null;
            });
        }
    }
}