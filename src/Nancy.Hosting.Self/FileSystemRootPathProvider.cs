using System;
using System.IO;
using System.Reflection;

namespace Nancy.Hosting.Self
{
    public class FileSystemRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            var assembly = Assembly.GetEntryAssembly();

            return assembly == null ? Environment.CurrentDirectory : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }
}