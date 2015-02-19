namespace Nancy.Diagnostics
{
    using Nancy.Bootstrapper;

    public interface IDiagnostics
    {
        /// <summary>
        /// Initialise diagnostics
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        void Initialize(IPipelines pipelines);
    }
}