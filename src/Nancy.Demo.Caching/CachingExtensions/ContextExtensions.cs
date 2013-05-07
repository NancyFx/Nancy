namespace Nancy.Demo.Caching.CachingExtensions
{
    public static class ContextExtensions
    {
        public const string OUTPUT_CACHE_TIME_KEY = "OUTPUT_CACHE_TIME";

        /// <summary>
        /// Enable output caching for this route
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="seconds">Seconds to cache for</param>
        public static void EnableOutputCache(this NancyContext context, int seconds)
        {
            context.Items[OUTPUT_CACHE_TIME_KEY] = seconds;
        }

        /// <summary>
        /// Disable the output cache for this route
        /// </summary>
        /// <param name="context">Current context</param>
        public static void DisableOutputCache(this NancyContext context)
        {
            context.Items.Remove(OUTPUT_CACHE_TIME_KEY);
        }
    }
}