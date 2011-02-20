namespace NancyCachineDemo
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Nancy;

    public class CachingBootstrapper : DefaultNancyBootstrapper
    {
        private const int CACHE_SECONDS = 30;

        private Dictionary<string, Tuple<DateTime, Response>> cachedResponses = new Dictionary<string, Tuple<DateTime, Response>>();

        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);

            this.PreRequestHooks += CheckCache;

            this.PostRequestHooks += SetCache;
        }

        /// <summary>
        /// Check to see if we have a cache entry.
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Request or null</returns>
        public Response CheckCache(NancyContext context)
        {
            Tuple<DateTime, Response> cacheEntry = null;

            if (this.cachedResponses.TryGetValue(context.Request.Uri, out cacheEntry))
            {
                if (cacheEntry.Item1.AddSeconds(CACHE_SECONDS) > DateTime.Now)
                {
                    context.Items["Cached"] = true;
                    return cacheEntry.Item2;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the current response to the cache
        /// Only stores by Uri and stores the response in a dictionary.
        /// Do not use this as an actual cache :-)
        /// </summary>
        /// <param name="context">Current context</param>
        public void SetCache(NancyContext context)
        {
            // If response was returned from the cache, don't store it again
            if (context.Items.ContainsKey("Cached"))
            {
                return;
            }

            if (context.Response.StatusCode == HttpStatusCode.OK)
            {
                var cachedResponse = new CachedResponse(context.Response);

                this.cachedResponses[context.Request.Uri] = new Tuple<DateTime, Response>(DateTime.Now, cachedResponse);

                context.Response = cachedResponse;
            }
        }
    }
}