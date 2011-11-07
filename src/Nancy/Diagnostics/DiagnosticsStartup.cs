namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    using Nancy.Bootstrapper;

    public class DiagnosticsStartup : IStartup
    {
        private IDiagnosticSessions sessionProvider;

        public DiagnosticsStartup(IDiagnosticSessions sessionProvider)
        {
            this.sessionProvider = sessionProvider;
        }

        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get
            {
                return null;
            }
        }

        public void Initialize(IPipelines pipelines)
        {
            pipelines.AfterRequest += Hooks.GetSaveHook(this.sessionProvider);
        }
    }
}