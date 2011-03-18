using Nancy.Bootstrapper;

namespace Nancy.Responses
{
    using System;
    using System.IO;

    public class GenericFileResponse : Response
    {
        public GenericFileResponse(string filePath)
        {
            InitializeGenericFileResonse(filePath, InferMimeType());
        }        

        public GenericFileResponse(string filePath, string contentType)
        {
            InitializeGenericFileResonse(filePath, contentType);
        }

        private string InferMimeType()
        {
            //NancyBootstrapperLocator.Bootstrapper
            return "application/octet-stream";
        }

        private static Action<Stream> GetFileContent(string filePath)
        {
            return stream =>
            {
                using (var file = File.OpenRead(filePath))
                {
                    var buffer = new byte[4096];
                    var read = 1;
                    while (read > 0)
                    {                                   
                        read = file.Read(buffer, 0, buffer.Length);
                        stream.Write(buffer, 0, read);
                    }
                }
            };
        }

        private void InitializeGenericFileResonse(string filePath, string contentType)
        {
            if (string.IsNullOrEmpty(filePath) ||
                !File.Exists(filePath) ||
                !Path.HasExtension(filePath))
            {
                this.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                this.Contents = GetFileContent(filePath);
                this.ContentType = contentType;
                this.StatusCode = HttpStatusCode.OK;
            }
        }
    }
}
