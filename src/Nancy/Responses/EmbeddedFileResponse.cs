namespace Nancy.Responses
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
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

            var content =
                    GetResourceContent(assembly, resourcePath, name);

            if (content != null)
            {
                this.WithHeader("ETag", GenerateETag(content));
                content.Seek(0, SeekOrigin.Begin);
            }

            this.Contents = stream =>
            {
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

            return assembly.GetManifestResourceStream(resourceName);
        }

        private static string GetFileNameFromResourceName(string resourcePath, string resourceName)
        {
            return resourceName.Replace(resourcePath, string.Empty).Substring(1);
        }

        private static string GenerateETag(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return string.Concat("\"", ByteArrayToString(hash), "\"");
            }
        }

        private static string ByteArrayToString(byte[] data)
        {
            var output = new StringBuilder(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                output.Append(data[i].ToString("X2"));
            }

            return output.ToString();
        }
    }
}