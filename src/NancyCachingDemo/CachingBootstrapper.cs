namespace NancyCachingDemo
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Nancy;

    public class CachingBootstrapper : DefaultNancyBootstrapper
    {
        private const int CACHE_SECONDS = 30;

        private Dictionary<string, Tuple<DateTime, Response, int>> cachedResponses = new Dictionary<string, Tuple<DateTime, Response, int>>();

        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);

            this.BeforeRequest += CheckCache;

            this.AfterRequest += SetCache;
        }

        /// <summary>
        /// Check to see if we have a cache entry - if we do, see if it has expired or not,
        /// if it hasn't then return it, otherwise return null;
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Request or null</returns>
        public Response CheckCache(NancyContext context)
        {
            Tuple<DateTime, Response, int> cacheEntry = null;

            if (this.cachedResponses.TryGetValue(context.Request.Uri, out cacheEntry))
            {
                if (cacheEntry.Item1.AddSeconds(cacheEntry.Item3) > DateTime.Now)
                {
                    return cacheEntry.Item2;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the current response to the cache if required
        /// Only stores by Uri and stores the response in a dictionary.
        /// Do not use this as an actual cache :-)
        /// </summary>
        /// <param name="context">Current context</param>
        public void SetCache(NancyContext context)
        {
            if (context.Response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            object cacheSecondsObject;
            if (!context.Items.TryGetValue(CachingExtensions.ContextExtensions.OUTPUT_CACHE_TIME_KEY, out cacheSecondsObject))
            {
                return;
            }

            int cacheSeconds;
            if (!int.TryParse(cacheSecondsObject.ToString(), out cacheSeconds))
            {       
                return;
            }

            var cachedResponse = new CachedResponse(context.Response);

            this.cachedResponses[context.Request.Uri] = new Tuple<DateTime, Response, int>(DateTime.Now, cachedResponse, cacheSeconds);

            context.Response = cachedResponse;
        }
    }
}