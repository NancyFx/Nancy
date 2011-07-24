namespace Nancy.Hosting.Wcf
{
    using System;
    using System.IO;
    using System.Reflection;

    public class FileSystemRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            var assembly = Assembly.GetEntryAssembly();

            return assembly == null ? Environment.CurrentDirectory : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }
}