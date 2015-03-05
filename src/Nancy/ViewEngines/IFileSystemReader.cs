namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Defines the functionality for retrieving information about views that are stored on the file system.
    /// </summary>
    public interface IFileSystemReader
    {
        /// <summary>
        /// Gets information about view that are stored in folders below the applications root path.
        /// </summary>
        /// <param name="path">The path of the folder where the views should be looked for.</param>
        /// <param name="supportedViewExtensions">A list of view extensions to look for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing view locations and contents readers.</returns>
        IEnumerable<Tuple<string, Func<StreamReader>>> GetViewsWithSupportedExtensions(string path, IEnumerable<string> supportedViewExtensions);

        /// <summary>
        /// Gets the last modified time for the file specified
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <returns>Time the file was last modified</returns>
        DateTime GetLastModified(string filename);

        /// <summary>
        /// Gets information about specific views that are stored in folders below the applications root path.
        /// </summary>
        /// <param name="path">The path of the folder where the views should be looked for.</param>
        /// <param name="viewName">Name of the view to search for</param>
        /// <param name="supportedViewExtensions">A list of view extensions to look for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing view locations and contents readers.</returns>
        IEnumerable<Tuple<string, Func<StreamReader>>> GetViewsWithSupportedExtensions(string path, string viewName, IEnumerable<string> supportedViewExtensions);
    }
}