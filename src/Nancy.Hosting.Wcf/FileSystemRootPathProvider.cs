namespace Nancy.Hosting.Wcf
{
    using System;

    public class FileSystemRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Environment.CurrentDirectory;
        }
    }
}