namespace Nancy.Hosting.Self
{
    using System;
    using System.IO;
    using System.Reflection;

    public class FileSystemRootPathProvider : IRootPathProvider
    {
        private readonly Lazy<string> rootPath = new Lazy<string>(ExtractRootPath);

        public string GetRootPath()
        {
            return this.rootPath.Value;
        }

        private static string ExtractRootPath()
        {
            var assembly =
                Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var location = assembly.Location;

            return Path.GetDirectoryName(location);
        }
    }
}
