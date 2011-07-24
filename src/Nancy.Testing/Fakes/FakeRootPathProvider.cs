namespace Nancy.Testing.Fakes
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Fake root path provider - set the static <see cref="RootPath"/> property
    /// </summary>
    public class FakeRootPathProvider : IRootPathProvider
    {
        private static string rootPath;
        public static string RootPath
        {
            get { return rootPath; }
            set { rootPath = value; }
        }

        static FakeRootPathProvider()
        {
            var assembly = Assembly.GetEntryAssembly();

            rootPath = assembly == null ? Environment.CurrentDirectory : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        /// <summary>
        /// Returns the root folder path of the current Nancy application.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the path of the root folder.</returns>
        string IRootPathProvider.GetRootPath()
        {
            return RootPath;
        }
    }
}