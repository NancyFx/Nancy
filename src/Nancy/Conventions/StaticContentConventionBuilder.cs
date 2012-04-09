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
        private static readonly Regex PathReplaceRegex = new Regex(@"[/\\]", RegexOptions.Compiled);
		
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
            // !!! BREAKING CHANGE. ALL CONTENTPATH SEPARATORS HAS TO BE FORWARD SLASHES
            // THAT WAY BOTH REQUESTED AND CONTENT PATHS ARE DECLARED THE SAME WAY
            // DESPITE WHAT PLATFORM NANCY IS RUNNING ON. PROPER SEPERATOR REPLACEMENT
            // AUTOMATICALLY TAKES PLACE ONCE THE PATH HAS BEEN TRANSFORMED !!!

            return (ctx, root) =>{

                var path =
                    ctx.Request.Path.TrimStart(new[] { '/' });

                var fileName = 
                    Path.GetFileName(ctx.Request.Path);

                if (string.IsNullOrEmpty(fileName))
                {
                    return null;
                }

                var encodedRequestedPath = 
                    GetEncodedRequestedPath(requestedPath);

                // CAN I CLEAN UP THIS METHOD
                var pathWithoutFilename = 
                    GetPathWithoutFilename(fileName, path);

                if (!pathWithoutFilename.StartsWith(encodedRequestedPath, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                if(contentPath != null)
                {
                    contentPath = GetSafeContentPath(contentPath);
                    //contentPath = GetSafeContentPath(PathReplaceRegex.Replace(contentPath, Path.DirectorySeparatorChar.ToString()));
                }

                var responseFactory =
                    ResponseFactoryCache.GetOrAdd(path, BuildContentDelegate(ctx, root, encodedRequestedPath.TrimStart(new[] {'/'}), contentPath ?? requestedPath, allowedExtensions));

                return responseFactory.Invoke();
            };
        }

        private static string GetSafeContentPath(string contentPath)
        {
            if (contentPath == null)
            {
                return null;
            }

            return contentPath.Equals("/") ? string.Empty : contentPath;
            //return contentPath.Equals(Path.DirectorySeparatorChar.ToString()) ? string.Empty : contentPath;
        }

        private static string GetEncodedRequestedPath(string requestedPath)
        {
            return (!requestedPath.EndsWith("/")) ?
                string.Concat(requestedPath, "/") :
                requestedPath;
        }

        private static string GetPathWithoutFilename(string fileName, string path)
        {
            var pathWithoutFilename = path
                .Replace(fileName, string.Empty);
                //.TrimStart(new[] {'/'});

            return (!pathWithoutFilename.EndsWith("/")) ?
                string.Concat(pathWithoutFilename, "/") :
                pathWithoutFilename;
        }

        private static Func<string, Func<Response>> BuildContentDelegate(NancyContext context, string applicationRootPath, string requestedPath, string contentPath, string[] allowedExtensions)
        {
            return requestPath =>
            {
                context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] Attempting to resolve static content '", requestPath, "'")));
                var extension = Path.GetExtension(requestPath);

                if (string.IsNullOrEmpty(extension))
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine("[StaticContentConventionBuilder] The requested file did not contain a file extension."));
                    return () => null;
                }

                if (allowedExtensions.Length != 0 && !allowedExtensions.Any(e => string.Equals(e, extension, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The requested extension '", extension, "' does not match any of the valid extensions for the convention '", string.Join(",", allowedExtensions), "'")));
                    return () => null;
                }

                requestPath = PathReplaceRegex
                    .Replace(GetSafeRequestPath(requestPath, requestedPath, contentPath), Path.DirectorySeparatorChar.ToString())
                    .TrimStart(Path.DirectorySeparatorChar);

                var fileName = 
                    Path.GetFullPath(Path.Combine(applicationRootPath, requestPath));

                var contentRootPath = Path.Combine(
                    applicationRootPath,
                    PathReplaceRegex.Replace(contentPath, Path.DirectorySeparatorChar.ToString()));

                if (!IsWithinContentFolder(contentRootPath, fileName))
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The request '", fileName, "' is trying to access a path outside the content folder '", contentPath, "'")));
                    return () => null;
                }

                if (!File.Exists(fileName))
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The requested file '", fileName, "' does not exist")));
                    return () => null;
                }

                context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] Returning file '", fileName, "'")));
                return () => new GenericFileResponse(fileName);
            };
        }

        private static string GetSafeRequestPath(string requestPath, string requestedPath, string contentPath)
        {
            if (requestedPath.Equals("/"))
            {
                return string
                    .Concat(contentPath, Path.DirectorySeparatorChar, requestPath)
                    .TrimStart(Path.DirectorySeparatorChar);
            }

            var expression = 
                new Regex(requestedPath.TrimEnd(new[] {'/'}), RegexOptions.IgnoreCase);

            return expression.Replace(requestPath, Regex.Escape(contentPath), 1);
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