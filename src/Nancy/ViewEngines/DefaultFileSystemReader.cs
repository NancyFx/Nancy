namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Default implementation for retrieving informationa about views that are stored on the file system.
    /// </summary>
    public class DefaultFileSystemReader : IFileSystemReader
    {
        /// <summary>
        /// Gets information about view that are stored in folders below the applications root path.
        /// </summary>
        /// <param name="path">The path of the folder where the views should be looked for.</param>
        /// <param name="supportedViewExtensions">A list of view extensions to look for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing view locations and contents readers.</returns>
        public IEnumerable<Tuple<string, Func<StreamReader>>> GetViewsWithSupportedExtensions(string path, IEnumerable<string> supportedViewExtensions)
        {
            return supportedViewExtensions
                .SelectMany(extension => GetFilenames(path, extension))
                .Distinct()
                .Select(file => new Tuple<string, Func<StreamReader>>(file, () => new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))))
                .ToList();
        }

        private static IEnumerable<string> GetFilenames(string path, string extension)
        {
            return Directory.GetFiles(path, string.Concat("*.", extension), SearchOption.AllDirectories);
        }
    }
}