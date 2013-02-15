namespace Nancy.Helpers
{
    using System;
    using System.Linq;

    /// <summary>
    /// Helper class for caching related functions
    /// </summary>
    public static class CacheHelpers
    {
        /// <summary>
        /// Returns whether to return a not modified response, based on the etag and last modified date
        /// of the resource, and the current nancy context
        /// </summary>
        /// <param name="etag">Current resource etag, or null</param>
        /// <param name="lastModified">Current resource last modified, or null</param>
        /// <param name="context">Current nancy context</param>
        /// <returns>True if not modified should be sent, false otherwise</returns>
        public static bool ReturnNotModified(string etag, DateTime? lastModified, NancyContext context)
        {
            if (context == null || context.Request == null)
            {
                return false;
            }

            var requestEtag = context.Request.Headers.IfNoneMatch.FirstOrDefault();
            var requestDate = context.Request.Headers.IfModifiedSince;

            if (requestEtag != null && !string.IsNullOrEmpty(etag) && requestEtag.Equals(etag, StringComparison.Ordinal))
            {
                return true;
            }

            if (requestDate.HasValue && lastModified.HasValue && ((int)(lastModified.Value - requestDate.Value).TotalSeconds) <= 0)
            {
                return true;
            }

            return false;
        }

    }
}