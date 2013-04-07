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
        /// conventions.Static((File, Directory) =>
        /// {
        ///     File["/page.js"] = "page.js";
        ///     Directory["/images"] = "images";
        /// });
        /// </summary>
        /// <param name="staticConventions">The callback method allows you to describe the static content</param>
        public static void Static(this NancyConventions conventions, Action<StaticFileContent, StaticDirectoryContent> staticConventions)
        {
            staticConventions(new StaticFileContent(conventions), new StaticDirectoryContent(conventions));
        }
    }
}
