namespace Nancy.Diagnostics
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class EmbeddedFileResponse : Response
    {
        public EmbeddedFileResponse(Assembly assembly, string resourcePath, string name)
        {
            this.ContentType = MimeTypes.GetMimeType(name);
            this.StatusCode = HttpStatusCode.OK;

            this.Contents = stream =>
            {
                var content = 
                    GetResourceContent(assembly, resourcePath, name);

                content.CopyTo(stream);
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