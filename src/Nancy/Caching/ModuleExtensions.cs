
namespace Nancy.Caching
{
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class ModuleExtensions
    {
        public static void IncludeETag(this NancyModule module)
        {
            module.After.AddItemToEndOfPipeline(IncludeETag);
        }

        private static void IncludeETag(NancyContext context)
        {
            var response = context.Response;

            if (response.Headers.ContainsKey("ETag"))
            {
                return;
            }

            var ms = new MemoryStream();
            response.Contents(ms);
            var data = ms.ToArray();
            ms.Seek(0, SeekOrigin.Begin);
            response.Contents = ms.CopyTo;

            response.Headers["ETag"] = GenerateETag(data);
        }

        private static string GenerateETag(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(data);
                return ByteArrayToString(hash);
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
