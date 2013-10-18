namespace Nancy.Conventions
{
    using System;

    /// <summary>
    /// Nancy static convention helper
    /// </summary>
    public static class StaticContentHelper
    {
        /// <summary>
        /// Extension method for NancyConventions
        /// 
        /// conventions.MapStaticContent((File, Directory) =>
        /// {
        ///     File["/page.js"] = "page.js";
        ///     Directory["/images"] = "images";
        /// });
        /// </summary>
        /// <param name="conventions">The conventions to add to.</param>
        /// <param name="staticConventions">The callback method allows you to describe the static content</param>
        public static void MapStaticContent(this NancyConventions conventions, Action<StaticFileContent, StaticDirectoryContent> staticConventions)
        {
            staticConventions(new StaticFileContent(conventions), new StaticDirectoryContent(conventions));
        }
    }
}
