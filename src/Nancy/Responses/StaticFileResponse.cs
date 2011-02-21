namespace Nancy.Responses
{
    using System;
    using System.IO;
    using System.Net;
    using System.Web.Hosting;

    public class StaticFileResponse : Response
    {
        public StaticFileResponse(string filePath, string contentType)
        {
            this.StatusCode = HttpStatusCode.NotFound;

            var expandedFilePath = GetExpandedFilePath(filePath);
            if (IsValidFilePath(expandedFilePath))
            {
                this.Contents = GetFileContent(expandedFilePath);
                this.ContentType = contentType;
                this.StatusCode = HttpStatusCode.OK;
            }
        }

        private static bool IsValidFilePath(string filePath)
        {
            return !(string.IsNullOrEmpty(filePath) || !File.Exists(filePath) || !Path.HasExtension(filePath));
        }

        private static string GetExpandedFilePath(string filePath)
        {
            return HostingEnvironment.MapPath(filePath);
        }

        private static Action<Stream> GetFileContent(string filePath)
        {
            return stream =>
            {
                using (var reader = new StreamReader(filePath))
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(reader.ReadToEnd());
                    writer.Flush();
                }
            };
        }
    }
}