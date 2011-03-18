using System.IO.Compression;

namespace Nancy.Responses
{
    using System;
    using System.IO;

    public class GZipResponse : GenericFileResponse
    {
        private const string GZIP_SUFFIX = ".gz"; 

        // TODO: This should try to intelligently get the mime type, assuming "application/octet-stream" is not very good.
        public GZipResponse(string filePath) : this(filePath, "application/octet-stream") {}

        public GZipResponse(string filePath, string contentType) : base(filePath + GZIP_SUFFIX, contentType)
        {
            string gzipped = filePath + GZIP_SUFFIX;
            if (!File.Exists(gzipped) || File.GetLastWriteTime(filePath) > File.GetLastWriteTime(gzipped)) { GZipFile(filePath, gzipped); }
        }

        private static void GZipFile(string filePath, string gzipped)
        { 
            using (FileStream inFile = File.OpenRead(filePath))
            {                
                    using (FileStream outFile = File.Create(gzipped))
                    {
                          using (GZipStream Compress = new GZipStream(outFile, CompressionMode.Compress))
                          {
                              inFile.CopyTo(Compress);
                          }
                      }
             }
        }
    }
}
