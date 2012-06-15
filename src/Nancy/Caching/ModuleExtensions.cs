
namespace Nancy.Caching
{
    using System;
    using System.Globalization;
    using System.Linq;

    public static class ModuleExtensions
    {
        public static void CheckForIfNonMatch(this NancyModule module)
        {
            module.After.AddItemToEndOfPipeline(CheckForInNonMatch);
        }

        public static void CheckForIfModifiedSince(this NancyModule module)
        {
            module.After.AddItemToEndOfPipeline(CheckForIfModifiedSince);
        }

        private static void CheckForInNonMatch(NancyContext context)
        {
            var request = context.Request;
            var response = context.Response;

            string responseETag;
            if (response.Headers.TryGetValue("ETag", out responseETag))
            {
                if (request.Headers.IfNoneMatch.Contains(responseETag))
                {
                    context.Response = HttpStatusCode.NotModified;
                }
            }
        }

        private static void CheckForIfModifiedSince(NancyContext context)
        {
            var request = context.Request;
            var response = context.Response;

            string responseLastModified;
            if (response.Headers.TryGetValue("Last-Modified", out responseLastModified))
            {
                DateTime lastModified;

                if (request.Headers.IfModifiedSince.HasValue && DateTime.TryParseExact(responseLastModified, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out lastModified))
                {
                    if (lastModified <= request.Headers.IfModifiedSince.Value)
                    {
                        context.Response = HttpStatusCode.NotModified;
                    }
                }
            }
        }
    }
}
