namespace Nancy.Testing
{
    using System.IO;

    public static class ContextExtensions
    {
        private const string DOCUMENT_WRAPPER_KEY_NAME = "@@@@DOCUMENT_WRAPPER@@@@";

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