using Nancy.Bootstrapper;

namespace Nancy.Diagnostics
{
    public class DisabledDiagnostics : IDiagnostics
    {
        public void Initialize(IPipelines pipelines)
        {
            // Do nothing :-)
        }
    }
}