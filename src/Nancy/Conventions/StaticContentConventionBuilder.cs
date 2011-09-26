namespace Nancy.Conventions
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using Responses;

    /// <summary>
    /// Helper class for defining directory-based conventions for static contents.
    /// </summary>
    public class StaticContentConventionBuilder
    {
        private static readonly ConcurrentDictionary<string, Func<Response>> ResponseFactoryCache;

        static StaticContentConventionBuilder()
        {
            ResponseFactoryCache = new ConcurrentDictionary<string, Func<Response>>();
        }

        /// <summary>
        /// Adds a directory-based convention for static convention.
        /// </summary>
        /// <param name="requestPath">The path that should be matched with the request.</param>
        /// <param name="contentPath">The path to where the content is stored in your application, relative to the root. If this is <see langword="null" /> then it will be the same as <paramref name="requestPath"/>.</param>
        /// <param name="allowedExtensions">A list of extensions that is valid for the conventions. If not supplied, all extensions are valid.</param>
        /// <returns>A <see cref="GenericFileResponse"/> instance for the requested static contents if it was found, otherwise <see langword="null"/>.</returns>
        public static Func<NancyContext, string, Response> AddDirectory(string requestPath, string contentPath = null, params string[] allowedExtensions)
        {
            return (ctx, root) =>
            {
                var path =
                    ctx.Request.Path.TrimStart(new[] { '/' });

                if (!path.StartsWith(requestPath, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                var responseFactory =
                    ResponseFactoryCache.GetOrAdd(path, BuildContentDelegate(root, contentPath ?? requestPath, allowedExtensions));

                return responseFactory.Invoke();
            };
        }

        private static Func<string, Func<Response>> BuildContentDelegate(string applicationRootPath, string contentPath, string[] allowedExtensions)
        {
            return requestPath =>
            {
                var extension = Path.GetExtension(requestPath);

                if (string.IsNullOrEmpty(extension))
                {
                    return () => null;
                }

                if (allowedExtensions.Length != 0 && !allowedExtensions.Any(e => string.Equals(e, extension, StringComparison.OrdinalIgnoreCase)))
                {
                    return () => null;
                }

                if(!requestPath.StartsWith(contentPath, StringComparison.OrdinalIgnoreCase))
                {
                    requestPath = String.Concat(contentPath, requestPath.Substring(requestPath.IndexOf("/")));
                }

                var fileName = Path.Combine(applicationRootPath, requestPath);

                if (!File.Exists(fileName))
                {
                    return () => null;
                }

                return () => new GenericFileResponse(fileName);
            };
        }
    }
}