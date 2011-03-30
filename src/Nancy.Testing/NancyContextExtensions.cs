namespace Nancy.Testing
{
    using System.IO;

    /// <summary>
    /// Defines extensions for the <see cref="NancyContext"/> type.
    /// </summary>
    public static class NancyContextExtensions
    {
        private const string DOCUMENT_WRAPPER_KEY_NAME = "@@@@DOCUMENT_WRAPPER@@@@";

        /// <summary>
        /// Returns the HTTP response body, of the specified <see cref="NancyContext"/>, wrapped in an <see cref="DocumentWrapper"/> instance.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> instance that the HTTP response body should be retrieved from.</param>
        /// <returns>A <see cref="DocumentWrapper"/> instance, wrapping the HTTP response body of the context.</returns>
        public static DocumentWrapper DocumentBody(this NancyContext context)
        {
            // We only really want to generate this once, so we'll stick it in the context
            // This isn't ideal, but we don't want to hide the guts of the context from the
            // tests this will have to do.
            if (context.Items.ContainsKey(DOCUMENT_WRAPPER_KEY_NAME))
            {
                return (DocumentWrapper)context.Items[DOCUMENT_WRAPPER_KEY_NAME];
            }

            DocumentWrapper wrapper;
            using (var contentsStream = new MemoryStream())
            {
                context.Response.Contents.Invoke(contentsStream);
                contentsStream.Position = 0;
                wrapper = new DocumentWrapper(contentsStream);
            }

            context.Items[DOCUMENT_WRAPPER_KEY_NAME] = wrapper;

            return wrapper;
        }
    }
}