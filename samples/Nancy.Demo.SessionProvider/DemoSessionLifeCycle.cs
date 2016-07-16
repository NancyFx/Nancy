

namespace Nancy.Demo.SessionProvider
{
    using Nancy.Session;
    using Nancy.Bootstrapper;

    /// <summary>
    /// This is a sample of the basic implementation for a session lifecycle. Nancy use auto discover and load the session lifecycle. If no custom implementation is found, the <cref=DefaultNancySessionLifeCycle></cref> will be used.
    /// </summary>
    class DemoSessionLifeCycle
    {
        public class DefaultNancySessionLifecycle : ISessionLifecycle
        {
            static SessionProvider _sessionProvider;
            
            public void Initialise(IPipelines pipelines)
            {
            }
            
            public void LoadSessionProvider(SessionProvider provider)
            {
                _sessionProvider = provider;
            }
            
            public Response OnStart(NancyContext ctx)
            {
                if (_sessionProvider == null || ctx.Request == null)
                {
                    return null;
                }

                var session = _sessionProvider.Load(ctx);
                if (session == null)
                {
                    session = _sessionProvider.New(ctx);
                }

                ctx.Request.Session = session;

                return null;
            }
            
            public void OnEnd(NancyContext ctx)
            {
                if (_sessionProvider != null && ctx.Request != null)
                {
                    if (ctx.Request.Session != null && ctx.Request.Session.HasChanged)
                    {
                        _sessionProvider.Save(ctx.Request.Session, ctx);
                    }
                }
            }
        }
    }
}
