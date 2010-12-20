namespace Nancy.Tests.Extensions
{
    using System;
    using System.IO;
    using Cookies;

    public static class ResponseExtensions
    {
        public static string GetStringContentsFromResponse(this Response response)
        {
            var memory = new MemoryStream();
            response.Contents.Invoke(memory);
            memory.Position = 0;
            using (var reader = new StreamReader(memory))
            {
                return reader.ReadToEnd();
            }
        }

        public static void ShouldEqual(this INancyCookie cookie, string name, string value, DateTime? expires, string domain, string path)
        {
            cookie.Name.ShouldEqual(name);
            cookie.Value.ShouldEqual(value);
            cookie.Expires.ShouldEqual(expires);
            cookie.Domain.ShouldEqual(domain);
            cookie.Path.ShouldEqual(path);
        }
    }
}