namespace Nancy.Embedded.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Extension methods to aid adding embedded static content into conventions
    /// </summary>
    public static class StaticContentsConventionsExtensions
    {
        /// <summary>
        /// Adds a directory-based convention for embedded static resources
        /// </summary>
        /// <param name="conventions">Conventions in which static content will be added</param>
        /// <param name="requestedPath">The path that should be matched with the request</param>
        /// <param name="contentPath">The path to where the content is stored in your application, relative to the root. If this is <see langword="null" /> then it will be the same as <paramref name="requestedPath"/>.</param>
        /// <param name="allowedExtensions">A list of extensions that is valid for the conventions. If not supplied, all extensions are valid.</param>
        public static void AddEmbeddedDirectory(this IList<Func<NancyContext, string, Response>> conventions, string requestedPath, string contentPath = null, params string[] allowedExtensions)
        {
            AddEmbeddedDirectory(conventions, requestedPath, Assembly.GetCallingAssembly(), contentPath, allowedExtensions);
        }

        /// <summary>
        /// Adds a directory-based convention for embedded static resources
        /// </summary>
        /// <param name="conventions">Conventions in which static content will be added</param>
        /// <param name="requestedPath">The path that should be matched with the request</param>
        /// <param name="assembly">The assembly that contains the embedded static content</param>
        /// <param name="contentPath">The path to where the content is stored in your application, relative to the root. If this is <see langword="null" /> then it will be the same as <paramref name="requestedPath"/>.</param>
        /// <param name="allowedExtensions">A list of extensions that is valid for the conventions. If not supplied, all extensions are valid.</param>
        public static void AddEmbeddedDirectory(this IList<Func<NancyContext, string, Response>> conventions, string requestedPath, Assembly assembly, string contentPath = null, params string[] allowedExtensions)
        {
            conventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory(requestedPath, assembly, contentPath, allowedExtensions));
        }
    }
}
