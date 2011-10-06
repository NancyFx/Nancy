namespace Nancy.Conventions
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
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
        /// <param name="requestedPath">The path that should be matched with the request.</param>
        /// <param name="contentPath">The path to where the content is stored in your application, relative to the root. If this is <see langword="null" /> then it will be the same as <paramref name="requestedPath"/>.</param>
        /// <param name="allowedExtensions">A list of extensions that is valid for the conventions. If not supplied, all extensions are valid.</param>
        /// <returns>A <see cref="GenericFileResponse"/> instance for the requested static contents if it was found, otherwise <see langword="null"/>.</returns>
        public static Func<NancyContext, string, Response> AddDirectory(string requestedPath, string contentPath = null, params string[] allowedExtensions)
        {
            return (ctx, root) =>
            {
                var path =
                    ctx.Request.Path.TrimStart(new[] { '/' });

                if (!path.StartsWith(requestedPath, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                var responseFactory =
                    ResponseFactoryCache.GetOrAdd(path, BuildContentDelegate(root, requestedPath, contentPath ?? requestedPath, allowedExtensions));

                return responseFactory.Invoke();
            };
        }

        private static Func<string, Func<Response>> BuildContentDelegate(string applicationRootPath, string requestedPath, string contentPath, string[] allowedExtensions)
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

                requestPath = Regex.Replace(requestPath, requestedPath, Regex.Escape(contentPath), RegexOptions.IgnoreCase);

                var fileName = Path.GetFullPath(Path.Combine(applicationRootPath, requestPath));

                var contentRootPath = Path.Combine(applicationRootPath, contentPath);
                if (!IsWithinContentFolder(contentRootPath, fileName) || !File.Exists(fileName))
                {
                    return () => null;
                }

                return () => new GenericFileResponse(fileName);
            };
        }

        /// <summary>
        /// Returns whether the given filename is contained within the content folder
        /// </summary>
        /// <param name="contentRootPath">Content root path</param>
        /// <param name="fileName">Filename requested</param>
        /// <returns>True if contained within the content root, false otherwise</returns>
        private static bool IsWithinContentFolder(string contentRootPath, string fileName)
        {
            return fileName.StartsWith(contentRootPath, StringComparison.Ordinal);
        }
    }
}