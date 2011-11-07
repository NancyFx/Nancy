namespace Nancy.Demo.Hosting.Aspnet
{
    using System;
    using System.Linq;
    using System.Text;

    using Nancy.Diagnostics;

    public class DiagnosticsModule : NancyModule
    {
        private readonly IDiagnosticSessions sessionProvider;

        public DiagnosticsModule(IDiagnosticSessions sessionProvider)
            :base("/diags")
        {
            this.sessionProvider = sessionProvider;

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
        }
    }
}