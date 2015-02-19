namespace Nancy.Responses
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Nancy.Helpers;

    /// <summary>
    /// A response representing a file.
    /// </summary>
    /// <remarks>If the response contains an invalid file (not found, empty name, missing extension and so on) the status code of the response will be set to <see cref="HttpStatusCode.NotFound"/>.</remarks>
    public class GenericFileResponse : Response
    {
        /// <summary>
        /// Represents a list of "base paths" where it is safe to
        /// serve files from.
        /// Attempting to server a file outside of these safe paths
        /// will fail with a 404.
        /// </summary>
        public static IList<string> SafePaths { get; set; }

        /// <summary>
        ///  Size of buffer for transmitting file. Default size 4 Mb
        /// </summary>
        public static int BufferSize = 4 * 1024 * 1024;

        static GenericFileResponse()
        {
            SafePaths = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFileResponse"/> for the file specified
        /// by the <paramref name="filePath"/> parameter.
        /// </summary>
        /// <param name="filePath">The name of the file, including path relative to the root of the application, that should be returned.</param>
        /// <remarks>The <see cref="MimeTypes.GetMimeType"/> method will be used to determine the mimetype of the file and will be used as the content-type of the response. If no match if found the content-type will be set to application/octet-stream.</remarks>
        public GenericFileResponse(string filePath) :
            this (filePath, MimeTypes.GetMimeType(filePath))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFileResponse"/> for the file specified
        /// by the <paramref name="filePath"/> parameter.
        /// </summary>
        /// <param name="filePath">The name of the file, including path relative to the root of the application, that should be returned.</param>
        /// <remarks>The <see cref="MimeTypes.GetMimeType"/> method will be used to determine the mimetype of the file and will be used as the content-type of the response. If no match if found the content-type will be set to application/octet-stream.</remarks>
        /// <param name="context">Current context</param>
        public GenericFileResponse(string filePath, NancyContext context)
            : this(filePath, MimeTypes.GetMimeType(filePath), context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFileResponse"/> for the file specified
        /// by the <paramref name="filePath"/> parameter and the content-type specified by the <paramref name="contentType"/> parameter.
        /// </summary>
        /// <param name="filePath">The name of the file, including path relative to the root of the application, that should be returned.</param>
        /// <param name="contentType">The content-type of the response.</param>
        /// <param name="context">Current context</param>
        public GenericFileResponse(string filePath, string contentType, NancyContext context = null)
        {
            InitializeGenericFileResponse(filePath, contentType, context);
        }

        /// <summary>
        /// Gets the filename of the file response
        /// </summary>
        /// <value>A string containing the name of the file.</value>
        public string Filename { get; protected set; }

        private static Action<Stream> GetFileContent(string filePath, long length)
        {
            return stream =>
            {
                using (var file = File.OpenRead(filePath))
                {
                    file.CopyTo(stream, (int)(length < BufferSize ? length : BufferSize));
                }
            };
        }

        static bool IsSafeFilePath(string rootPath, string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            var fullPath = Path.GetFullPath(filePath);

            return fullPath.StartsWith(Path.GetFullPath(rootPath), StringComparison.OrdinalIgnoreCase);
        }

        private void InitializeGenericFileResponse(string filePath, string contentType, NancyContext context)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                StatusCode = HttpStatusCode.NotFound;
                return;
            }
            if (SafePaths == null || SafePaths.Count == 0)
            {
                throw new InvalidOperationException("No SafePaths defined.");
            }
            foreach (var rootPath in SafePaths)
            {
                string fullPath;
                if (Path.IsPathRooted(filePath))
                {
                    fullPath = filePath;
                }
                else
                {
                    fullPath = Path.Combine(rootPath, filePath);
                }

                if (IsSafeFilePath(rootPath, fullPath))
                {
                    this.Filename = Path.GetFileName(fullPath);

                    this.SetResponseValues(contentType, fullPath, context);

                    return;
                }
            }

            StatusCode = HttpStatusCode.NotFound;
        }

        private void SetResponseValues(string contentType, string fullPath, NancyContext context)
        {
            // TODO - set a standard caching time and/or public?
            var fi = new FileInfo(fullPath);

            var lastWriteTimeUtc = fi.LastWriteTimeUtc;
            var etag = string.Concat("\"", lastWriteTimeUtc.Ticks.ToString("x"), "\"");
            var lastModified = lastWriteTimeUtc.ToString("R");
            var length = fi.Length;

            if (CacheHelpers.ReturnNotModified(etag, lastWriteTimeUtc, context))
            {
                this.StatusCode = HttpStatusCode.NotModified;
                this.ContentType = null;
                this.Contents = NoBody;

                return;
            }

            this.Headers["ETag"] = etag;
            this.Headers["Last-Modified"] = lastModified;
            this.Headers["Content-Length"] = length.ToString();

            if (length > 0)
            {
                this.Contents = GetFileContent(fullPath, length);
            }

            this.ContentType = contentType;
            this.StatusCode = HttpStatusCode.OK;
        }
    }
}
