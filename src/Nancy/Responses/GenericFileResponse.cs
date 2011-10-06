namespace Nancy.Responses
{
    using System;
    using System.IO;

    /// <summary>
    /// A response representing a file. 
    /// </summary>
    /// <remarks>If the response contains an invalid file (not found, empty name, missing extension and so on) the status code of the response will be set to <see cref="HttpStatusCode.NotFound"/>.</remarks>
    public class GenericFileResponse : Response
    {
        public static string RootPath { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFileResponse"/> for the file specified
        /// by the <param name="filePath" /> parameter.
        /// </summary>
        /// <param name="filePath">The name of the file, including path relative to the root of the application, that should be returned.</param>
        /// <remarks>The <see cref="MimeTypes.GetMimeType"/> method will be used to determine the mimetype of the file and will be used as the content-type of the response. If no match if found the content-type will be set to application/octet-stream.</remarks>
        public GenericFileResponse(string filePath) : 
            this (filePath, MimeTypes.GetMimeType(filePath))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFileResponse"/> for the file specified
        /// by the <param name="filePath" /> parameter and the content-type specified by the <param name="contentType" /> parameter.
        /// </summary>
        /// <param name="filePath">The name of the file, including path relative to the root of the application, that should be returned.</param>
        /// <param name="contentType">The content-type of the response.</param>
        public GenericFileResponse(string filePath, string contentType)
        {
            InitializeGenericFileResonse(filePath, contentType);
        }

        /// <summary>
        /// Gets the filename of the file response
        /// </summary>
        /// <value>A string containing the name of the file.</value>
        public string Filename { get; protected set; }

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

        private static bool IsSafeFilePath(string rootPath, string filePath)
        {
            if (string.IsNullOrEmpty(RootPath))
            {
                return false;
            }

            if (!Path.HasExtension(filePath))
            {
                return false;
            }

            if (!File.Exists(filePath))
            {
                return false;
            }

            var fullPath = Path.GetFullPath(filePath);

            return fullPath.StartsWith(rootPath, StringComparison.Ordinal);
        }

        private void InitializeGenericFileResonse(string filePath, string contentType)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                this.StatusCode = HttpStatusCode.NotFound;
                return;
            }

            var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(RootPath, filePath);

            if (!IsSafeFilePath(RootPath, fullPath))
            {
                this.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                this.Filename = Path.GetFileName(fullPath);

                var fi = new FileInfo(fullPath);
                // TODO - set a standard caching time and/or public?
                this.Headers["ETag"] = fi.LastWriteTimeUtc.Ticks.ToString("x");
                this.Headers["Last-Modified"] = fi.LastWriteTimeUtc.ToString("R");
                this.Contents = GetFileContent(fullPath);
                this.ContentType = contentType;
                this.StatusCode = HttpStatusCode.OK;
            }
        }
    }
}
