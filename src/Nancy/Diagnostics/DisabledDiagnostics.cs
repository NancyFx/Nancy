namespace Nancy.Diagnostics
{
    using Nancy.Bootstrapper;

    public class DisabledDiagnostics : IDiagnostics
    {
        public void Initialize(IPipelines pipelines)
        {
            // Do nothing :-)
        }
    }
}