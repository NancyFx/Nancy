namespace Nancy
{
    using Nancy.Bootstrapper;
    using Nancy.Session;

    /// <summary>
    /// <para>Default session lifecycle</para>
    /// <para>Sessions in this lifecycle per request follows: Load > Create (if not found) > Save</para>
    /// </summary>
    public class DefaultNancySessionLifecycle : ISessionLifecycle
    {
        private static SessionProvider sessionProvider;

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="pipelines">pipeline</param>
        public void Initialise(IPipelines pipelines)
        {
        }

        /// <summary>
        /// Load session provider
        /// </summary>
        /// <param name="provider">Loaded session provider</param>
        public void LoadSessionProvider(SessionProvider provider)
        {
            sessionProvider = provider;
        }
        /// <summary>
        /// Start of pipeline
        /// </summary>
        /// <param name="ctx">Nancy Context</param>
        /// <returns>Response with initialised session</returns>
        public Response OnStart(NancyContext ctx)
        {
            if (sessionProvider == null || ctx.Request == null)
            {
                return null;
            }

            var session = sessionProvider.Load(ctx);
            if (session == null)
            {
                session = sessionProvider.New(ctx);
            }

            ctx.Request.Session = session;

            return null;
        }

        /// <summary>
        /// End of pipeline
        /// </summary>
        /// <param name="ctx">Nancy Context</param>
        public void OnEnd(NancyContext ctx)
        {
            if (sessionProvider != null && ctx.Request != null)
            {
                if (ctx.Request.Session != null && ctx.Request.Session.HasChanged)
                {
                    sessionProvider.Save(ctx.Request.Session, ctx);
                }
            }
        }
    }
}