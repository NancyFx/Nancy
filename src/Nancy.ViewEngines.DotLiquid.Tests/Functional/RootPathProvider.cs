namespace Nancy.ViewEngines.DotLiquid.Tests.Functional
{
    using Testing;

    public class RootPathProvider : IRootPathProvider
    {
        /// <summary>
        /// Returns the root folder path of the current Nancy application.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the path of the root folder.</returns>
        public string GetRootPath()
        {
            var assemblyPath =
                System.IO.Path.GetDirectoryName(typeof(RootPathProvider).Assembly.CodeBase).Replace(@"file:\", string.Empty);

            return PathHelper.GetParent(assemblyPath, 2);
        }
    }
}