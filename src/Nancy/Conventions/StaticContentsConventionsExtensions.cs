namespace Nancy.Conventions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods to adding static content conventions.
    /// </summary>
    public static class StaticContentsConventionsExtensions
    {
        /// <summary>
        /// Adds a directory-based convention for static convention.
        /// </summary>
        /// <param name="conventions">The conventions to add to.</param>
        /// <param name="requestedPath">The path that should be matched with the request.</param>
        /// <param name="contentPath">The path to where the content is stored in your application, relative to the root. If this is <see langword="null" /> then it will be the same as <paramref name="requestedPath"/>.</param>
        /// <param name="allowedExtensions">A list of extensions that is valid for the conventions. If not supplied, all extensions are valid.</param>
        public static void AddDirectory(this IList<Func<NancyContext, string, Response>> conventions, string requestedPath, string contentPath = null, params string[] allowedExtensions)
        {
            conventions.Add(StaticContentConventionBuilder.AddDirectory(requestedPath, contentPath, allowedExtensions));
        }

        /// <summary>
        /// Adds a directory-based convention for static convention.
        /// </summary>
        /// <param name="conventions">The conventions to add to.</param>
        /// <param name="requestedFile">The file that should be matched with the request.</param>
        /// <param name="contentFile">The file that should be served when the requested path is matched.</param>
        public static void AddFile(this IList<Func<NancyContext, string, Response>> conventions, string requestedFile, string contentFile)
        {
            conventions.Add(StaticContentConventionBuilder.AddFile(requestedFile, contentFile));
        }
    }
}