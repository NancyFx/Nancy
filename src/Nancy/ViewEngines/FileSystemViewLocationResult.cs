namespace Nancy.ViewEngines
{
    using System;
    using System.IO;

    /// <summary>
    /// View location result for file system based views.
    /// Supports detecting if the contents have changed since it
    /// was last read.
    /// </summary>
    public class FileSystemViewLocationResult : ViewLocationResult
    {
        private readonly IFileSystemReader fileSystem;

        private readonly string fileName;

        private DateTime lastUpdated;

        private readonly Func<TextReader> fileContents;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocationResult"/> class.
        /// </summary>
        /// <param name="location">The location of where the view was found.</param>
        /// <param name="name">The name of the view.</param>
        /// <param name="extension">The file extension of the located view.</param>
        /// <param name="contents">A <see cref="TextReader"/> that can be used to read the contents of the located view.</param>
        /// <param name="fullFilename">Full filename of the file</param>
        /// <param name="fileSystem">An <see cref="IFileSystemReader"/> instance that should be used when retrieving view information from the file system.</param>
        public FileSystemViewLocationResult(string location, string name, string extension, Func<TextReader> contents, string fullFilename, IFileSystemReader fileSystem)
        {
            this.fileSystem = fileSystem;
            this.Location = location;
            this.Name = name;
            this.Extension = extension;
            this.fileContents = contents;
            this.Contents = this.GetContents;
            this.fileName = fullFilename;
        }

        /// <summary>
        /// Gets a value indicating whether the current item is stale
        /// </summary>
        /// <returns>True if stale, false otherwise</returns>
        public override bool IsStale()
        {
            return this.lastUpdated != this.fileSystem.GetLastModified(this.fileName);
        }

        /// <summary>
        /// Wraps the real contents delegate to set the last modified date first
        /// </summary>
        /// <returns>TextReader to read the file</returns>
        private TextReader GetContents()
        {
            this.lastUpdated = this.fileSystem.GetLastModified(this.fileName);

            return this.fileContents.Invoke();
        }
    }
}