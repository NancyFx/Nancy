namespace Nancy.Diagnostics.Modules
{
    using System.Linq;

    public class InteractiveModule : DiagnosticModule
    {
        private readonly IDiagnosticSessions sessionProvider;

        private readonly IInteractiveDiagnostics interactiveDiagnostics;

        public InteractiveModule(IDiagnosticSessions sessionProvider, IInteractiveDiagnostics interactiveDiagnostics)
            :base ("/interactive")
        {
            this.sessionProvider = sessionProvider;
            this.interactiveDiagnostics = interactiveDiagnostics;

            Get["/"] = _ => View["InteractiveDiagnostics"];

            Get["/providers"] = _ =>
                {
                    var providers = this.interactiveDiagnostics
                                        .AvailableDiagnostics
                                        .Select(p => new { p.Name, p.Description })
                                        .ToArray();

                    return Response.AsJson(providers);
                };
        } 

    }
}