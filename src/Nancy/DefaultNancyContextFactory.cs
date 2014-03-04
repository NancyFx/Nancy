namespace Nancy
{
    using Nancy.Culture;
    using Nancy.Diagnostics;

    /// <summary>
    /// Creates NancyContext instances
    /// </summary>
    public class DefaultNancyContextFactory : INancyContextFactory
    {
        private readonly ICultureService cultureService;
        private readonly IRequestTraceFactory requestTraceFactory;

        /// <summary>
        /// Creates a new instance of the <see cref="DefaultNancyContextFactory"/> class.
        /// </summary>
        /// <param name="cultureService">An <see cref="ICultureService"/> instance.</param>
        /// <param name="requestTraceFactory">An <see cref="IRequestTraceFactory"/> instance.</param>
        public DefaultNancyContextFactory(ICultureService cultureService, IRequestTraceFactory requestTraceFactory)
        {
            this.cultureService = cultureService;
            this.requestTraceFactory = requestTraceFactory;
        }

        /// <summary>
        /// Create a new <see cref="NancyContext"/> instance.
        /// </summary>
        /// <returns>A <see cref="NancyContext"/> instance.</returns>
        public NancyContext Create(Request request)
        {
            var context =
                new NancyContext();

            context.Trace = this.requestTraceFactory.Create(request);
            context.Request = request;
            context.Culture = this.cultureService.DetermineCurrentCulture(context);

            // Move this to DefaultRequestTrace.
            context.Trace.TraceLog.WriteLog(s => s.AppendLine("New Request Started"));

            return context;
        }
    }
}