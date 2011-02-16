namespace Nancy.ViewEngines
{
    using System.IO;

    public class ViewLocationResult
    {
        public ViewLocationResult(string location, string extension, TextReader contents)
        {
            this.Location = location;
            this.Extension = extension;
            this.Contents = contents;
        }

        public TextReader Contents { get; private set; }

        /// <summary>
        /// Gets the extension of the view that was located.
        /// </summary>
        /// <value>A <see cref="string"/> containing the extension of the view that was located.</value>
        /// <remarks>The extension should not contain a leading dot.</remarks>
        public string Extension { get; private set; }
        
        public string Location { get; private set; }
    }
}