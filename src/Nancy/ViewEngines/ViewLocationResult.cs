namespace Nancy.ViewEngines
{
    using System;
    using System.IO;

    /// <summary>
    /// Contains the result of an attempt to locate a view.
    /// </summary>
    public class ViewLocationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocationResult"/> class.
        /// </summary>
        /// <param name="location">The location of where the view was found.</param>
        /// <param name="name">The name of the view.</param>
        /// <param name="extension">The file extension of the located view.</param>
        /// <param name="contents">A <see cref="TextReader"/> that can be used to read the contents of the located view.</param>
        public ViewLocationResult(string location, string name, string extension, Func<TextReader> contents)
        {
            this.Location = location;
            this.Name = name;
            this.Extension = extension;
            this.Contents = contents;
        }

        /// <summary>
        /// Gets a function that produces a reader for retrieving the contents of the view.
        /// </summary>
        /// <value>A <see cref="Func{T}"/> instance that can be used to produce a reader for retrieving the contents of the view.</value>
        public Func<TextReader> Contents { get; set; }

        /// <summary>
        /// Gets the extension of the view that was located.
        /// </summary>
        /// <value>A <see cref="string"/> containing the extension of the view that was located.</value>
        /// <remarks>The extension should not contain a leading dot.</remarks>
        public string Extension { get; private set; }

        /// <summary>
        /// Gets the location of where the view was found.
        /// </summary>
        /// <value>A <see cref="string"/> containing the location of the view.</value>
        /// <remarks>This value is returned by the <see cref="IViewSourceProvider"/> instance that located the view and is in the path format that is uses.</remarks>
        public string Location { get; private set; }

        /// <summary>
        /// Gets the full name of the view that was found
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the view.</value>
        public string Name { get; private set; }
    }
}