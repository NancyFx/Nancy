namespace Nancy.MimeType
{
    /// <summary>
    /// A static class wrapper around the IMimeTypeRegistry functionality.
    /// </summary>
    public static class MimeTypeUtils
    {
        private static readonly IMimeTypeRegistry reg = new DefaultMimeTypeRegistry();

        /// <summary>
        /// Gets the mime type for the specified extension.
        /// </summary>
        /// <param name="extension">The extension whose mime type we are to infer</param>
        /// <returns>The inferred mime type for the specified extension</returns>
        public static string GetMimeTypeFor(string extension) { return reg.GetMimeTypeFor(extension); }
    }
}