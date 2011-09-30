namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Defines the functionality for retriving information about views that are stored on the file system.
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
    }
}