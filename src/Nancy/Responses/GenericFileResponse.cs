namespace Nancy.Responses
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    public class GenericFileResponse : Response
    {
        public GenericFileResponse(string filePath) :
            this(filePath, MimeTypes.GetMimeType(filePath), new Dictionary<string, string>())
        {
        }

        public GenericFileResponse(string filePath, IDictionary<string, string> headers) :
            this(filePath, MimeTypes.GetMimeType(filePath), headers)
        {
        }

        public GenericFileResponse(string filePath, string contentType) :
            this(filePath, contentType, new Dictionary<string, string>())
        {

        }

        public GenericFileResponse(Stream stream) :
            this(stream, MimeTypes.GetMimeType(""), new Dictionary<string, string>() { })
        {
        }

        public GenericFileResponse(Stream stream, IDictionary<string, string> headers) :
            this(stream, MimeTypes.GetMimeType(""), headers)
        {
        }

        public GenericFileResponse(string filePath, string contentType, IDictionary<string, string> headers)
        {
            InitializeGenericFileResonse(filePath, contentType, headers);
        }

        public GenericFileResponse(Stream stream, string contentType, IDictionary<string, string> headers)
        {
            InitializeGenericFileResonse(stream, contentType, headers);
        }


        private static Action<Stream> GetFileContent(string filePath)
        {
            return stream =>
            {
                using (var file = File.OpenRead(filePath))
                {
                    var buffer = new byte[4096];
                    var read = -1;
                    while (read != 0)
                    {
                        read = file.Read(buffer, 0, buffer.Length);
                        stream.Write(buffer, 0, read);
                    }
                }
            };
        }

        private void InitializeGenericFileResonse(string filePath, string contentType, IDictionary<string, string> headers)
        {
            if (string.IsNullOrEmpty(filePath) ||
                !File.Exists(filePath) ||
                !Path.HasExtension(filePath))
            {
                this.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                var fi = new FileInfo(filePath);
                // TODO - set a standard caching time and/or public?
                this.Headers["ETag"] = fi.LastWriteTimeUtc.Ticks.ToString("x");
                this.Headers["Last-Modified"] = fi.LastWriteTimeUtc.ToString("R");
                //Add headers in dictionary
                foreach (var header in headers)
                {
                    this.Headers[header.Key] = header.Value;
                }
                this.Contents = GetFileContent(filePath);
                this.ContentType = contentType;
                this.StatusCode = HttpStatusCode.OK;
            }
        }
        private void InitializeGenericFileResonse(Stream stream, string contentType, IDictionary<string, string> headers)
        {
            if (stream.Length == 0)
            {
                this.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                // TODO - set a standard caching time and/or public?
                this.Headers["ETag"] = DateTime.Now.Ticks.ToString("x");
                this.Headers["Last-Modified"] = DateTime.Now.ToString("R");
                //Add headers in dictionary
                foreach (var header in headers)
                {
                    this.Headers[header.Key] = header.Value;
                }
                this.Contents = GetFileContent(stream);
                this.ContentType = contentType;
                this.StatusCode = HttpStatusCode.OK;
            }
        }

        private Action<Stream> GetFileContent(Stream memStream)
        {
            return stream =>
            {
                var buffer = new byte[4096];
                var read = -1;
                while (read != 0)
                {
                    read = memStream.Read(buffer, 0, buffer.Length);
                    stream.Write(buffer, 0, read);
                }
            };
        }
    }
}
