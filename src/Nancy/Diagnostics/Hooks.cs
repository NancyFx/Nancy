namespace Nancy.Diagnostics
{
    using System;
    using System.Linq;

    using Bootstrapper;

    public static class Hooks
    {
        public static Action<NancyContext> GetSaveHook(IDiagnosticSessions sessionProvider)
        {
            return ctx =>
                {
                    if (!StaticConfiguration.EnableDiagnostics)
                    {
                        return;
                    }

                    if (ctx.Request == null || ctx.Response == null)
                    {
                        return;
                    }

                    string sessionId;
                    if (!ctx.Request.Cookies.TryGetValue("NancyDiagnosticsSession", out sessionId))
                    {
                        sessionId = sessionProvider.CreateSession().ToString();
                    }
                    var sessionGuid = new Guid(sessionId);

                    if (!sessionProvider.GetSessions().Any(s => s.Id == sessionGuid))
                    {
                        sessionGuid = sessionProvider.CreateSession();
                    }

                    sessionProvider.AddRequestDiagnosticToSession(sessionGuid, ctx);

                    ctx.Response.AddCookie("NancyDiagnosticsSession", sessionGuid.ToString());

                    return;
                };
        }
    }
}