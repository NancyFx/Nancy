using Nancy.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.Session
{
    public class DefaultNancySessionLifecycle : ISessionLifecycle
    {
        static ISessionProvider _sessionProvider;

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
        public void LoadSessionProvider(ISessionProvider provider)
        {
            _sessionProvider = provider;
        }
        /// <summary>
        /// Start of pipeline
        /// </summary>
        /// <param name="ctx">Nancy Context</param>
        /// <returns>Response with initialised session</returns>
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
        /// <summary>
        /// End of pipeline
        /// </summary>
        /// <param name="ctx">Nancy Context</param>
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