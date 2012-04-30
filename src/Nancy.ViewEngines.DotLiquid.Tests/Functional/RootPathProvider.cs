namespace Nancy.ViewEngines.DotLiquid.Tests.Functional
{
    using System;
    using Testing;

    public class RootPathProvider : IRootPathProvider
    {
        /// <summary>
        /// Returns the root folder path of the current Nancy application.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the path of the root folder.</returns>
        public string GetRootPath()
        {
            var assemblyFilePath =
                new Uri(typeof (RootPathProvider).Assembly.CodeBase).LocalPath;

            var assemblyPath =
                System.IO.Path.GetDirectoryName(assemblyFilePath);

            return PathHelper.GetParent(assemblyPath, 2);
        }
    }
}