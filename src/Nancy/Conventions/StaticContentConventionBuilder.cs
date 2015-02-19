using Nancy.Diagnostics;

namespace Nancy.Conventions
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Nancy.Helpers;
    using Nancy.Responses;

    /// <summary>
    /// Helper class for defining directory-based conventions for static contents.
    /// </summary>
    public class StaticContentConventionBuilder
    {
        private static readonly ConcurrentDictionary<ResponseFactoryCacheKey, Func<NancyContext, Response>> ResponseFactoryCache;
        private static readonly Regex PathReplaceRegex = new Regex(@"[/\\]", RegexOptions.Compiled);

        static StaticContentConventionBuilder()
        {
            ResponseFactoryCache = new ConcurrentDictionary<ResponseFactoryCacheKey, Func<NancyContext, Response>>();
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
            if (!requestedPath.StartsWith("/"))
            {
                requestedPath = string.Concat("/", requestedPath);
            }

            return (ctx, root) =>
            {
                var path =
                    HttpUtility.UrlDecode(ctx.Request.Path);

                var fileName = GetSafeFileName(path);

                if (string.IsNullOrEmpty(fileName))
                {
                    return null;
                }

                var pathWithoutFilename =
                    GetPathWithoutFilename(fileName, path);

                if (!pathWithoutFilename.StartsWith(requestedPath, StringComparison.OrdinalIgnoreCase))
                {
                    (ctx.Trace.TraceLog ?? new NullLog()).WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The requested resource '", path, "' does not match convention mapped to '", requestedPath, "'" )));
                    return null;
                }

                contentPath =
                    GetContentPath(requestedPath, contentPath);

                if (contentPath.Equals("/"))
                {
                    throw new ArgumentException("This is not the security vulnerability you are looking for. Mapping static content to the root of your application is not a good idea.");
                }

                var responseFactory =
                    ResponseFactoryCache.GetOrAdd(new ResponseFactoryCacheKey(path, root), BuildContentDelegate(ctx, root, requestedPath, contentPath, allowedExtensions));

                return responseFactory.Invoke(ctx);
            };
        }

        /// <summary>
        /// Adds a file-based convention for static convention.
        /// </summary>
        /// <param name="requestedFile">The file that should be matched with the request.</param>
        /// <param name="contentFile">The file that should be served when the requested path is matched.</param>
        public static Func<NancyContext, string, Response> AddFile(string requestedFile, string contentFile)
        {
            return (ctx, root) =>
            {

                var path =
                    ctx.Request.Path;

                if (!path.Equals(requestedFile, StringComparison.OrdinalIgnoreCase))
                {
                    ctx.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The requested resource '", path, "' does not match convention mapped to '", requestedFile, "'")));
                    return null;
                }

                var responseFactory =
                    ResponseFactoryCache.GetOrAdd(new ResponseFactoryCacheKey(path, root), BuildContentDelegate(ctx, root, requestedFile, contentFile, new string[] { }));

                return responseFactory.Invoke(ctx);
            };
        }

        private static string GetSafeFileName(string path)
        {
            try
            {
                return Path.GetFileName(path);
            }
            catch (Exception)
            {
            }

            return null;
        }

        private static string GetContentPath(string requestedPath, string contentPath)
        {
            contentPath =
                contentPath ?? requestedPath;

            if (!contentPath.StartsWith("/"))
            {
                contentPath = string.Concat("/", contentPath);
            }

            return contentPath;
        }

        private static Func<ResponseFactoryCacheKey, Func<NancyContext, Response>> BuildContentDelegate(NancyContext context, string applicationRootPath, string requestedPath, string contentPath, string[] allowedExtensions)
        {
            return pathAndRootPair =>
            {
                context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] Attempting to resolve static content '", pathAndRootPair, "'")));

                var extension =
                    Path.GetExtension(pathAndRootPair.Path);

                if (!string.IsNullOrEmpty(extension))
                {
                    extension = extension.Substring(1);
                }

                if (allowedExtensions.Length != 0 && !allowedExtensions.Any(e => string.Equals(e.TrimStart(new [] {'.'}), extension, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The requested extension '", extension, "' does not match any of the valid extensions for the convention '", string.Join(",", allowedExtensions), "'")));
                    return ctx => null;
                }

                var transformedRequestPath =
                    GetSafeRequestPath(pathAndRootPair.Path, requestedPath, contentPath);

                transformedRequestPath =
                    GetEncodedPath(transformedRequestPath);

                var fileName =
                    Path.GetFullPath(Path.Combine(applicationRootPath, transformedRequestPath));

                var contentRootPath =
                    Path.GetFullPath(Path.Combine(applicationRootPath, GetEncodedPath(contentPath)));

                if (!IsWithinContentFolder(contentRootPath, fileName))
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The request '", fileName, "' is trying to access a path outside the content folder '", contentPath, "'")));
                    return ctx => null;
                }

                if (!File.Exists(fileName))
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The requested file '", fileName, "' does not exist")));
                    return ctx => null;
                }

                context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] Returning file '", fileName, "'")));
                return ctx => new GenericFileResponse(fileName, ctx);
            };
        }

        private static string GetEncodedPath(string path)
        {
            return PathReplaceRegex.Replace(path.TrimStart(new[] { '/' }), Path.DirectorySeparatorChar.ToString());
        }

        private static string GetPathWithoutFilename(string fileName, string path)
        {
            var pathWithoutFileName =
                path.Replace(fileName, string.Empty);

            return (pathWithoutFileName.Equals("/")) ?
                pathWithoutFileName :
                pathWithoutFileName.TrimEnd(new[] {'/'});
        }

        private static string GetSafeRequestPath(string requestPath, string requestedPath, string contentPath)
        {
            var actualContentPath =
                (contentPath.Equals("/") ? string.Empty : contentPath);

            if (requestedPath.Equals("/"))
            {
                return string.Concat(actualContentPath, requestPath);
            }

            var expression =
                new Regex(Regex.Escape(requestedPath), RegexOptions.IgnoreCase);

            return expression.Replace(requestPath, actualContentPath, 1);
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

        /// <summary>
        /// Used to uniquely identify a request. Needed for when two Nancy applications want to serve up static content of the same
        /// name from within the same AppDomain.
        /// </summary>
        private class ResponseFactoryCacheKey : IEquatable<ResponseFactoryCacheKey>
        {
            private readonly string path;
            private readonly string rootPath;

            public ResponseFactoryCacheKey(string path, string rootPath)
            {
                this.path = path;
                this.rootPath = rootPath;
            }

            /// <summary>
            /// The path of the static content for which this response is being issued
            /// </summary>
            public string Path
            {
                get { return this.path; }
            }

            /// <summary>
            /// The root folder path of the Nancy application for which this response will be issued
            /// </summary>
            public string RootPath
            {
                get { return this.rootPath; }
            }

            public bool Equals(ResponseFactoryCacheKey other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return string.Equals(this.path, other.path) && string.Equals(this.rootPath, other.rootPath);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return Equals((ResponseFactoryCacheKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((this.path != null ? this.path.GetHashCode() : 0) * 397) ^ (this.rootPath != null ? this.rootPath.GetHashCode() : 0);
                }
            }
        }
    }
}
