namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    using Nancy.Bootstrapper;
    using Nancy.ModelBinding;

    public class DiagnosticsStartup : IStartup
    {
        private readonly IEnumerable<IDiagnosticsProvider> diagnosticProviders;

        private readonly IRootPathProvider rootPathProvider;

        private readonly IEnumerable<ISerializer> serializers;

        private readonly IRequestTracing requestTracing;

        public DiagnosticsStartup(IEnumerable<IDiagnosticsProvider> diagnosticProviders, IRootPathProvider rootPathProvider, IEnumerable<ISerializer> serializers, IRequestTracing requestTracing)
        {
            this.diagnosticProviders = diagnosticProviders;
            this.rootPathProvider = rootPathProvider;
            this.serializers = serializers;
            this.requestTracing = requestTracing;
        }

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
            DiagnosticsHook.Enable(pipelines, this.diagnosticProviders, this.rootPathProvider, this.serializers, this.requestTracing);
        }
    }
}