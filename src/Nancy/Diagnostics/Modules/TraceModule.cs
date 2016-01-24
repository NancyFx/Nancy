namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Linq;

    public class TraceModule : DiagnosticModule
    {
        private readonly IRequestTracing sessionProvider;

        public TraceModule(IRequestTracing sessionProvider)
            : base("/trace")
        {
            this.sessionProvider = sessionProvider;

            Get["/"] = async (_, __) =>
            {
                return View["RequestTracing"];
            };

            Get["/sessions"] = async (_, __) =>
            {
                return this.Response.AsJson(this.sessionProvider.GetSessions().Select(s => new { Id = s.Id }).ToArray());
            };

            Get["/sessions/{id}"] = async (ctx, __) =>
            {
                Guid id;
                if (!Guid.TryParse(ctx.Id, out id))
                {
                    return HttpStatusCode.NotFound;
                }

                var session =
                    this.sessionProvider.GetSessions().FirstOrDefault(s => s.Id == id);

                if (session == null)
                {
                    return HttpStatusCode.NotFound;
                }

                return this.Response.AsJson(session.RequestTraces.Select(t => new
                    {
                        t.RequestData.Method,
                        RequestUrl = t.RequestData.Url,
                        RequestContentType = t.RequestData.ContentType,
                        ResponseContentType = t.ResponseData.ContentType,
                        RequestHeaders = t.RequestData.Headers,
                        ResponseHeaders = t.ResponseData.Headers,
                        t.ResponseData.StatusCode,
                        Log = t.TraceLog.ToString().Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.None),
                    }).ToArray());
            };
        }
    }
}