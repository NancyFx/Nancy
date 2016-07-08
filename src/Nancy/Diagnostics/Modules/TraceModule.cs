namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Linq;

    /// <summary>
    /// Nancy module for request tracing. Part of diagnostics module.
    /// </summary>
    /// <seealso cref="Nancy.Diagnostics.DiagnosticModule" />
    public class TraceModule : DiagnosticModule
    {
        private readonly IRequestTracing sessionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceModule"/> class.
        /// </summary>
        /// <param name="sessionProvider">The session provider.</param>
        public TraceModule(IRequestTracing sessionProvider)
            : base("/trace")
        {
            this.sessionProvider = sessionProvider;

            Get("/", _ =>
            {
                return View["RequestTracing"];
            });

            Get("/sessions", _ =>
            {
                return this.Response.AsJson(this.sessionProvider.GetSessions().Select(s => new { Id = s.Id }).ToArray());
            });

            Get("/sessions/{id}", ctx =>
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
            });
        }
    }
}
