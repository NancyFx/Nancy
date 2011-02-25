namespace Nancy.Responses
{
    using System;
    using System.IO;
    
    public class StaticFileResponse : Response
    {
        public StaticFileResponse(string filePath, string contentType)
        {
            this.StatusCode = HttpStatusCode.NotFound;

            if (IsValidFilePath(filePath))
            {
                this.Contents = GetFileContent(filePath);
                this.ContentType = contentType;
                this.StatusCode = HttpStatusCode.OK;
            }
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

        private static bool IsValidFilePath(string filePath)
        {
            return !(string.IsNullOrEmpty(filePath) || !File.Exists(filePath) || !Path.HasExtension(filePath));
        }
    }
}