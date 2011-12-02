namespace Nancy.Demo.Hosting.Aspnet
{
    using System;
    using System.Linq;
    using System.Text;

    using Nancy.Diagnostics;
    using Nancy.Json;

    public class DiagnosticsModule : NancyModule
    {
        private readonly IDiagnosticSessions sessionProvider;

        private readonly IInteractiveDiagnostics interactiveDiagnostics;

        public DiagnosticsModule(IDiagnosticSessions sessionProvider, IInteractiveDiagnostics interactiveDiagnostics)
            :base("/diags")
        {
            this.sessionProvider = sessionProvider;
            this.interactiveDiagnostics = interactiveDiagnostics;

            Get["/"] = _ =>
                {
                    var responseBuilder = new StringBuilder();

                    var sessions = this.sessionProvider.GetSessions();
                    foreach (var diagnosticSession in sessions)
                    {
                        responseBuilder.AppendFormat("<a href='/diags/{0}'>{0}</a><br/>", diagnosticSession.Id);
                    }

                    return responseBuilder.ToString();
                };

            Get["/{Id}"] = ctx =>
                {
                    var sessionGuid = new Guid(ctx.Id);

                    var session = sessionProvider.GetSessions().First(s => s.Id == sessionGuid);

                    var responseBuilder = new StringBuilder();

                    foreach (var diag in session.RequestDiagnostics)
                    {
                        responseBuilder.AppendFormat(
                            "*******<br/>Request: {0}</br/>Type: {2}<br/>Log:<br/>{1}<br/>*******<br/>", diag.RequestUrl.Path, diag.TraceLog.ToString().Replace("\n", "<br/>"), diag.ResponseType);
                    }

                    return responseBuilder.ToString();
                };

            Get["/interactive"] = _ => View["interactive-diags", this.interactiveDiagnostics.AvailableDiagnostics];

            Get["/interactive/{name}"] = ctx => View["interactive-diags-methods", this.interactiveDiagnostics.AvailableDiagnostics.FirstOrDefault(id => id.Name.Equals(ctx.Name))];

            Post["/interactive/{name}/{method}"] = ctx =>
                {
                    var diag = this.interactiveDiagnostics.AvailableDiagnostics.FirstOrDefault(id => id.Name.Equals(ctx.name));
                    var method = diag.Methods.FirstOrDefault(m => m.MethodName.Equals(ctx.method, StringComparison.OrdinalIgnoreCase));

                    var result = this.interactiveDiagnostics.ExecuteDiagnostic(method, new object[] { });

                    var json = new JavaScriptSerializer().Serialize(result);

                    return View["interactive-diags-results", new { Json = json }];
                };

            Get["/interactive/template/{name}/{method}"] = ctx =>
                {
                    var diag = this.interactiveDiagnostics.AvailableDiagnostics.FirstOrDefault(id => id.Name.Equals(ctx.name));
                    var method = diag.Methods.FirstOrDefault(m => m.MethodName.Equals(ctx.method, StringComparison.OrdinalIgnoreCase));

                    var result = this.interactiveDiagnostics.GetTemplate(method);

                    return result == null ? HttpStatusCode.NotFound : (Response)result;
                };

        }
    }
}