using Nancy.Culture;

namespace Nancy
{
    /// <summary>
    /// Creates NancyContext instances
    /// </summary>
    public class DefaultNancyContextFactory : INancyContextFactory
    {
        private readonly ICultureService cultureService;

        public DefaultNancyContextFactory(ICultureService cultureService)
        {
            this.cultureService = cultureService;
        }

        /// <summary>
        /// Create a new NancyContext
        /// </summary>
        /// <returns>NancyContext instance</returns>
        public NancyContext Create(Request request)
        {
            var nancyContext = new NancyContext();
            
            nancyContext.Request = request;

            nancyContext.Culture = this.cultureService.DetermineCurrentCulture(nancyContext);

            nancyContext.Trace.TraceLog.WriteLog(s => s.AppendLine("New Request Started"));

            return nancyContext;
        }
    }
}