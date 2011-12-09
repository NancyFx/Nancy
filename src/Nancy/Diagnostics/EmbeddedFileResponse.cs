namespace Nancy.Diagnostics
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class EmbeddedFileResponse : Response
    {
        private static readonly byte[] ErrorText;

        static EmbeddedFileResponse()
        {
            ErrorText = Encoding.UTF8.GetBytes("NOT FOUND");
        }

        public EmbeddedFileResponse(Assembly assembly, string resourcePath, string name)
        {
            this.ContentType = MimeTypes.GetMimeType(name);
            this.StatusCode = HttpStatusCode.OK;

            this.Contents = stream =>
            {
                var content = 
                    GetResourceContent(assembly, resourcePath, name);

                if (content != null)
                {
                    content.CopyTo(stream);
                }
                else
                {
                    stream.Write(ErrorText, 0, ErrorText.Length);
                }
            };
        }

        private Stream GetResourceContent(Assembly assembly, string resourcePath, string name)
        {
            var resourceName = assembly
                .GetManifestResourceNames()
                .Where(x => GetFileNameFromResourceName(resourcePath, x).Equals(name, StringComparison.OrdinalIgnoreCase))
                .Select(x => GetFileNameFromResourceName(resourcePath, x))
                .FirstOrDefault();

            resourceName =
                string.Concat(resourcePath, ".", resourceName);

            return this.GetType().Assembly.GetManifestResourceStream(resourceName);
        }

        private static string GetFileNameFromResourceName(string resourcePath, string resourceName)
        {
            return resourceName.Replace(resourcePath, string.Empty).Substring(1);
        }
    }
}