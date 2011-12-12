namespace Nancy.Diagnostics.Modules
{
    using System;
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

            Get["/providers/{providerName}"] = ctx =>
                {
                    var provider =
                        this.interactiveDiagnostics.AvailableDiagnostics.FirstOrDefault(
                            d => string.Equals(d.Name, ctx.providerName, StringComparison.OrdinalIgnoreCase));

                    if (provider == null)
                    {
                        return HttpStatusCode.NotFound;
                    }

                    var methods = provider.Methods
                                          .Select(m => new { m.MethodName })
                                          .ToArray();

                    return Response.AsJson(methods);
                };
        } 

    }
}