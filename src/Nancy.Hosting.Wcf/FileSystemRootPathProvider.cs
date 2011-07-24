namespace Nancy.Hosting.Wcf
{
    using System.IO;
    using System.Reflection;

    public class FileSystemRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }
}