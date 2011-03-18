namespace Nancy.MimeType
{
    /// <summary>
    /// Used to retreive the associated mime-type for a given extension. 
    /// </summary>
    public interface IMimeTypeRegistry
    {
        /// <summary>
        /// Returns the inferred mime-type for the given extension.
        /// </summary>
        /// <param name="extension">The extension to infer the mime-type for</param>
        /// <returns>The inferred mime-type for the specified extension</returns>
        string GetMimeTypeFor(string extension);
    }
}