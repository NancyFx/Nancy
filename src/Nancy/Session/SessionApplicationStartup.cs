namespace Nancy.Session
{
    using System.Collections.Generic;
    using System.Linq;
    using Bootstrapper;

    /// <summary>
    /// Startup task to bind session functionality into the request pipeline
    /// </summary>
    public class SessionApplicationStartup : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.BeforeRequest.AddItemToEndOfPipeline(LoadSession);
            pipelines.AfterRequest.AddItemToEndOfPipeline(SaveSession);
        }

        /// <summary>
        /// Determine the user's session and load it from the <see cref="ISessionStore"/>
        /// </summary>
        /// <param name="ctx">Current <see cref="NancyContext"/></param>
        /// <returns>Always returns null. Processing of the request continues through the pipeline</returns>
        public static Response LoadSession(NancyContext ctx)
        {
            if (ctx.Request == null || ctx.SessionStore == null)
            {
                return null;
            }

            IDictionary<string, object> items;

            if (ctx.SessionStore.TryLoadSession(ctx.Request, out items))
            {
                ctx.Request.Session = new DynamicSession(items);
            }
            else
            {
                ctx.Request.Session = new DynamicSession();
            }
            
            return null;
        }

        /// <summary>
        /// Determine if the user's session has changed and save the changes to the <see cref="ISessionStores"/>
        /// </summary>
        /// <param name="ctx">The current <see cref="NancyContext"/></param>
        public static void SaveSession(NancyContext ctx)
        {
            if (ctx.Request == null || ctx.Request.Session == null || ctx.SessionStore == null || !ctx.Request.Session.HasChanged)
            {
                return;
            }

            ctx.SessionStore.SaveSession(ctx, ctx.Request.Session.GetValueDictionary());
        }
    }
}
