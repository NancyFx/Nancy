namespace Nancy.Demo.Caching
{
    using System;
    using System.Collections.Generic;

    using Nancy.Bootstrapper;
    using Nancy.Demo.Caching.CachingExtensions;
    using Nancy.TinyIoc;

    public class CachingBootstrapper : DefaultNancyBootstrapper
    {
        private const int CACHE_SECONDS = 30;

        private readonly Dictionary<string, Tuple<DateTime, Response, int>> cachedResponses = new Dictionary<string, Tuple<DateTime, Response, int>>();

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.BeforeRequest += CheckCache;

            pipelines.AfterRequest += SetCache;
        }

        /// <summary>
        /// Check to see if we have a cache entry - if we do, see if it has expired or not,
        /// if it hasn't then return it, otherwise return null;
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Request or null</returns>
        public Response CheckCache(NancyContext context)
        {
            Tuple<DateTime, Response, int> cacheEntry;

            if (this.cachedResponses.TryGetValue(context.Request.Path, out cacheEntry))
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
        /// Only stores by Path and stores the response in a dictionary.
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
            if (!context.Items.TryGetValue(ContextExtensions.OUTPUT_CACHE_TIME_KEY, out cacheSecondsObject))
            {
                return;
            }

            int cacheSeconds;
            if (!int.TryParse(cacheSecondsObject.ToString(), out cacheSeconds))
            {
                return;
            }

            var cachedResponse = new CachedResponse(context.Response);

            this.cachedResponses[context.Request.Path] = new Tuple<DateTime, Response, int>(DateTime.Now, cachedResponse, cacheSeconds);

            context.Response = cachedResponse;
        }
    }
}